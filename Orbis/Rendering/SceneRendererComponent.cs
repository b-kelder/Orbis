using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.Simulation;
using Orbis.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Renders a 3d representation of the scene, allowing the user to move a camera to view it.
    /// </summary>
    class SceneRendererComponent : DrawableGameComponent
    {
        /// <summary>
        /// Mode used to determine cell vertex colors.
        /// </summary>
        enum CellColorMode
        {
            OwnerColor,
            Elevation,
            Wetness,
            Temperature
        }

        /// <summary>
        /// Render data mapped to a specific cell.
        /// </summary>
        class CellMappedData
        {
            /// <summary>
            /// Index into cellMeshes to the mesh containing this cell's vertices.
            /// </summary>
            public int MeshIndex { get; set; }
            /// <summary>
            /// Indexes of this cell's vertices in the mesh.
            /// </summary>
            public List<int> VertexIndexes { get; set; }
            /// <summary>
            /// Reference to the RenderInstance that represents this cell's decoration.
            /// Can be null when no decoration is present.
            /// </summary>
            public KeyValuePair<string, int> Decoration { get; set; }
        }

        /// <summary>
        /// Render data mapped to a specific biome.
        /// </summary>
        class BiomeMappedData
        {
            /// <summary>
            /// Mesh to use for a hex in this biome.
            /// </summary>
            public Mesh HexMesh { get; set; }
            /// <summary>
            /// Mesh to use for default cell decorations in this biome.
            /// </summary>
            public string DefaultDecoration { get; set; }
        }

        /// <summary>
        /// Used for returning the result of GenerateMeshesFromScene.
        /// </summary>
        struct MeshGenerationResult
        {
            public List<Mesh> rawMeshes;
            public List<RenderableMesh> renderableMeshes;
            public Dictionary<Cell, CellMappedData> cellData;
        }

        private Orbis orbis;

        private Dictionary<string, BiomeMappedData> biomeMappedData = new Dictionary<string, BiomeMappedData>();
        private Dictionary<Cell, CellMappedData> cellMappedData = new Dictionary<Cell, CellMappedData>();
        private Dictionary<Civilization, Color> civColors = new Dictionary<Civilization, Color>();
        private List<RenderableMesh> cellMeshes = new List<RenderableMesh>();
        private List<RenderInstance> renderInstances = new List<RenderInstance>();

        private Dictionary<string, DecorationData> decorations = new Dictionary<string, DecorationData>();

        private Effect basicShader;
        private CellColorMode cellColorMode;
        private bool enableDecorations;

        private Camera camera;
        private float rotation;
        private float distance;
        private float angle;

        private Queue<RenderableMesh> meshUpdateQueue = new Queue<RenderableMesh>();
        private Task<MeshGenerationResult> meshTask;
        private Scene renderedScene;
        private AtlasModelLoader modelLoader;
        private DecorationManager decorationManager;

        private Random random;

        private bool atlasDebugEnabled;
        private RenderInstance atlasDebugInstance;

        /// <summary>
        /// Returns true if the renderer is ready to accept simulation updates.
        /// </summary>
        public bool ReadyForUpdate { get {
                return renderedScene != null && cellMappedData.Count > 0 && cellMeshes.Count > 0 && meshUpdateQueue.Count == 0;
            } }

        /// <summary>
        /// Maximum time we can spend in Update updating meshes.
        /// </summary>
        public float MaxUpdateTime { get; set; }
        /// <summary>
        /// Amount of active render instances.
        /// </summary>
        public int RenderInstanceCount { get { return renderInstances.Count; } }

        public SceneRendererComponent(Orbis game) : base(game)
        {
            MaxUpdateTime = 3;
            orbis = game;
            cellColorMode = CellColorMode.OwnerColor;
            enableDecorations = true;
            atlasDebugEnabled = false;
        }

        public override void Initialize()
        {
            // Camera stuff
            rotation = 0;
            distance = 20;
            angle = -60;
            camera = new Camera();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load shader, set up settings
            var black = Game.Content.Load<Texture2D>("Textures/black");
            basicShader = Game.Content.Load<Effect>("Shaders/BasicColorMapped");
            basicShader.CurrentTechnique = basicShader.Techniques["DefaultTechnique"];
            basicShader.Parameters["ColorMapTexture"].SetValue(black);
            basicShader.Parameters["ColorInfluence"].SetValue(1.0f);

            // Load models, decoration and biome data
            modelLoader = new AtlasModelLoader(2048, 2048, basicShader, Game.Content);
            var decorationData = orbis.Content.Load<XMLModel.DecorationCollection>("Config/Decorations");
            decorationManager = new DecorationManager(decorationData, modelLoader, GraphicsDevice);
            foreach(var data in decorationData.Decorations)
            {
                decorations.Add(data.Name, new DecorationData(decorationManager.GetDecorationMesh(data.Name), GraphicsDevice, 1));
            }
            var biomeData = orbis.Content.Load<XMLModel.BiomeCollection>("Config/Biomes");
            biomeMappedData = new Dictionary<string, BiomeMappedData>();
            foreach (var biome in biomeData.Biomes)
            {
                biomeMappedData.Add(biome.Name, new BiomeMappedData
                {
                    HexMesh = modelLoader.LoadModel(biome.HexModel.Name, biome.HexModel.Texture, biome.HexModel.ColorTexture).Mesh,
                    DefaultDecoration = biome.DefaultDecoration,
                });
            }

            modelLoader.FinializeLoading(GraphicsDevice);

            // Set up shader textures
            basicShader.Parameters["MainTexture"].SetValue(modelLoader.Material.Texture);
            basicShader.Parameters["ColorMapTexture"].SetValue(modelLoader.Material.ColorMap);

            if (atlasDebugEnabled)
            {
                // Atlas texture debugging
                Mesh mesh = null;
                using (var stream = TitleContainer.OpenStream("Content/Meshes/quad_up.obj"))
                {
                    mesh = ObjParser.FromStream(stream);
                }
                atlasDebugInstance = new RenderInstance
                {
                    mesh = new RenderableMesh(GraphicsDevice, mesh),
                    matrix = Matrix.CreateScale(10) * Matrix.CreateTranslation(0, 0, 30),
                };
                basicShader.Parameters["ColorMapTexture"].SetValue(black);
            }

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Check if new meshes have been generated by task
            if (meshTask != null && meshTask.Status == TaskStatus.RanToCompletion)
            {
                // Just in case this isn't the first map generated we must remove all cell decorations and existing terrain hex meshes
                // Terrain meshes must be disposed since they won't be used again
                foreach(var cellMesh in this.cellMeshes)
                {
                    cellMesh.Dispose();
                }
                // Cell decorations are in cellMappedData, so they will be overwritten. Their meshes will be reused so MUST NOT be disposed
                // Reset renderinstances
                this.renderInstances = new List<RenderInstance>();
                // Update main data sets
                var meshData = meshTask.Result;
                this.cellMappedData = meshData.cellData;
                this.cellMeshes = meshData.renderableMeshes;
                // Set cell decorations
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach(var cell in this.cellMappedData)
                {
                    var biomeData = biomeMappedData[cell.Key.Biome.Name];
                    if(biomeData.DefaultDecoration != null)
                    {
                        SetCellDecoration(cell.Key, cell.Value, biomeData.DefaultDecoration);
                    }
                }
                var set = new HashSet<RenderableMesh>();
                foreach(var dec in this.decorations)
                {
                    dec.Value.Update(set);
                }
                foreach(var mesh in set)
                {
                    meshUpdateQueue.Enqueue(mesh);
                }
                stopwatch.Stop();
                Debug.WriteLine("Took " + stopwatch.Elapsed.TotalMilliseconds + "ms to set decorations");
                meshTask = null;
            }


            // Update some vertex buffers if we have to without impact framerate too much
            if(meshUpdateQueue.Count > 0)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    var mesh = meshUpdateQueue.Dequeue();
                    mesh.UpdateVertexBuffer();
                } while(meshUpdateQueue.Count > 0 && stopwatch.Elapsed.TotalMilliseconds <= this.MaxUpdateTime);

                stopwatch.Stop();
            }

            // Camera movement
            var camMoveDelta = Vector3.Zero;
            float movementSpeed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds * (distance / 10);
            float rotationSpeed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float zoomSpeed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float scale = camera.OrthographicScale;

            if (orbis.Input.IsKeyHeld(Keys.LeftShift))
            {
                movementSpeed /= 5;
                rotationSpeed /= 5;
                zoomSpeed /= 5;
            }
            if (orbis.Input.IsKeyHeld(Keys.LeftControl))
            {
                movementSpeed *= 5;
                rotationSpeed *= 5;
                zoomSpeed *= 5;
            }
            if (orbis.Input.IsKeyHeld(Keys.Up))
            {
                angle -= rotationSpeed;
            }
            if (orbis.Input.IsKeyHeld(Keys.Down))
            {
                angle += rotationSpeed;
            }
            if (orbis.Input.IsKeyHeld(Keys.Left))
            {
                rotation -= rotationSpeed;
            }
            if (orbis.Input.IsKeyHeld(Keys.Right))
            {
                rotation += rotationSpeed;
            }
            if (orbis.Input.IsKeyHeld(Keys.OemPlus))
            {
                distance -= zoomSpeed;
                //scale -= speed;
            }
            if (orbis.Input.IsKeyHeld(Keys.OemMinus))
            {
                distance += zoomSpeed;
                //scale += speed;
            }

            // Mouse Camera movement
            if (orbis.Input.IsMouseHold(Engine.MouseButton.Right))
            {
                //Zooming in/out with the mousewheel
                if (orbis.Input.MouseScroll() != 0)
                {
                    distance -= orbis.Input.MouseScroll() / 10;
                }
                //Rotating the Camera if the mouse moved
                if (orbis.Input.MouseMove() != Point.Zero)
                {
                    rotation += orbis.Input.MouseMove().X / 2;
                    angle += orbis.Input.MouseMove().Y / 2;
                }
            }

            if (orbis.Input.IsKeyHeld(Keys.W))
            {
                camMoveDelta.Y += movementSpeed * 0.07f;
            }
            if (orbis.Input.IsKeyHeld(Keys.A))
            {
                camMoveDelta.X -= movementSpeed * 0.07f;
            }
            if (orbis.Input.IsKeyHeld(Keys.S))
            {
                camMoveDelta.Y -= movementSpeed * 0.07f;
            }
            if (orbis.Input.IsKeyHeld(Keys.D))
            {
                camMoveDelta.X += movementSpeed * 0.07f;
            }

            angle = MathHelper.Clamp(angle, -80, -5);
            distance = MathHelper.Clamp(distance, 1, 4000);

            //camera.OrthographicScale = MathHelper.Clamp(scale, 0.1f, 1000f);

            camera.LookTarget = camera.LookTarget + Vector3.Transform(camMoveDelta, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)));

            var camMatrix = Matrix.CreateTranslation(0, -distance, 0) *
               Matrix.CreateRotationX(MathHelper.ToRadians(angle)) *
               Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            camera.Position = Vector3.Transform(Vector3.Zero, camMatrix) + camera.LookTarget;


            // Recalculate renderinstances
            renderInstances = new List<RenderInstance>();
            foreach (var mesh in this.cellMeshes)
            {
                renderInstances.Add(new RenderInstance
                {
                    mesh = mesh,
                    matrix = Matrix.Identity
                });
            }
            foreach(var decoration in this.decorations)
            {
                renderInstances.AddRange(decoration.Value.GetActiveInstances());
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var graphics = orbis.Graphics;

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.Clear(Color.Aqua);

            // Required because SpriteBatch resets these
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            float aspectRatio = orbis.Graphics.PreferredBackBufferWidth / (float)orbis.Graphics.PreferredBackBufferHeight;
            Matrix viewMatrix = camera.CreateViewMatrix();
            Matrix projectionMatrix = camera.CreateProjectionMatrix(aspectRatio);

            // Create batches sorted by mesh, minimizes buffer switching?
            var meshBatches = new Dictionary<RenderableMesh, List<RenderInstance>>();
            foreach (var instance in renderInstances)
            {
                if (!meshBatches.ContainsKey(instance.mesh))
                {
                    meshBatches.Add(instance.mesh, new List<RenderInstance>());
                }
                meshBatches[instance.mesh].Add(instance);
            }
            if (atlasDebugEnabled)
            {
                meshBatches[atlasDebugInstance.mesh] = new List<RenderInstance>()
                {
                    atlasDebugInstance,
                };
            }

            // Draw batches
            foreach (var batch in meshBatches)
            {
                graphics.GraphicsDevice.Indices = batch.Key.IndexBuffer;
                graphics.GraphicsDevice.SetVertexBuffer(batch.Key.VertexBuffer);

                foreach (var instance in batch.Value)
                {
                    basicShader.Parameters["WorldViewProjection"].SetValue(instance.matrix * viewMatrix * projectionMatrix);

                    foreach (var pass in basicShader.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            0,
                            0,
                            instance.mesh.IndexBuffer.IndexCount);
                    }
                }
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Call when a new world is generated.
        /// </summary>
        /// <param name="scene">Scene containing new world</param>
        /// <param name="seed">Seed used to generate world</param>
        public async void OnNewWorldGenerated(Scene scene, int seed)
        {
            renderedScene = scene;
            random = new Random(seed);

            var colorRandom = new Random(seed);
            civColors = new Dictionary<Civilization, Color>();
            foreach (var civ in scene.Civilizations)
            {
                civColors.Add(civ, civ.Color);
            }


            // Await for a previous mesh generation to finish if it hasn't yet
            if (meshTask != null)
            {
                await meshTask;
            }
            meshTask = Task.Run(() => {
                return GenerateMeshesFromScene(scene);
            });

            // Set cam to sea level
            camera.LookTarget = new Vector3(camera.LookTarget.X, camera.LookTarget.Y, scene.Settings.SeaLevel);
        }

        private MeshGenerationResult GenerateMeshesFromScene(Scene scene)
        {
            var renderableMeshes = new List<RenderableMesh>();
            var rawMeshes = new List<Mesh>();
            var cellData = new Dictionary<Cell, CellMappedData>();

            // Use mesh combiners to get a bit more performant mesh for now
            var hexCombiner = new MeshCombiner();

            // Create world meshes
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int range = scene.WorldMap.Radius;

            for (int p = -range; p <= range; p++)
            {
                for (int q = -range; q <= range; q++)
                {
                    var cell = scene.WorldMap.GetCell(p, q);
                    if (cell == null)
                    {
                        continue;
                    }

                    var worldPoint = TopographyHelper.HexToWorld(new Point(p, q));
                    var position = new Vector3(
                        worldPoint,
                        (float)cell.Elevation);
                    // Cell color
                    var color = GetCellColor(cell);
                    var mesh = biomeMappedData[cell.Biome.Name].HexMesh;

                    // Make sea actually level
                    if (cell.IsWater && cell.Elevation < scene.Settings.SeaLevel)
                    {
                        position.Z = scene.Settings.SeaLevel;
                    }

                    int meshIndex = hexCombiner.Add(new MeshInstance
                    {
                        mesh = mesh,
                        matrix = Matrix.CreateTranslation(position),
                        tag = new Point(p, q),
                        color = color,
                        useColor = true,
                    });

                    // Register partial cell mapped data
                    var cellMappedData = new CellMappedData
                    {
                        MeshIndex = meshIndex
                    };
                    cellData[cell] = cellMappedData;
                }
            }

            // Combine meshes
            var meshList = hexCombiner.GetCombinedMeshes();
            for(int i = 0; i < meshList.Count; i++)
            {
                var renderable = new RenderableMesh(orbis.GraphicsDevice, meshList[i]);
                renderableMeshes.Add(renderable);

                Debug.WriteLine("Adding hex mesh");
            }
            // Finish cell mapped data
            foreach(var cell in cellData)
            {
                var mesh = meshList[cell.Value.MeshIndex];
                cell.Value.VertexIndexes = mesh.TagIndexMap[cell.Key.Coordinates];
            }

            stopwatch.Stop();
            Debug.WriteLine("Generated " + renderableMeshes.Count + " meshes in " + stopwatch.ElapsedMilliseconds + " ms");

            return new MeshGenerationResult {
                cellData = cellData,
                rawMeshes = rawMeshes,
                renderableMeshes = renderableMeshes
            };
        }

        /// <summary>
        /// Called to update the rendered representation of the scene.
        /// MAY NOT be put in a task.
        /// </summary>
        /// <param name="scene">Scene to update to</param>
        public void UpdateScene(Cell[] cells)
        {
            int updatedCells = 0;
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            var updatedMeshes = new HashSet<RenderableMesh>();
            foreach(var cell in cells)
            {
                if(cell == null) { continue; }
                var data = cellMappedData[cell];
                var mesh = cellMeshes[data.MeshIndex];
                foreach(var i in data.VertexIndexes)
                {
                    mesh.VertexData[i].Color = GetCellColor(cell);
                }
                updatedMeshes.Add(mesh);
                updatedCells++;

                if(cell.population >= renderedScene.DecorationSettings.LargePopulationThreshold)
                {
                    SetCellDecoration(cell, data, "Large Settlement");
                }
                else if(cell.population >= renderedScene.DecorationSettings.MediumPopulationThreshold)
                {
                    SetCellDecoration(cell, data, "Medium Settlement");
                }
                else if(cell.population >= renderedScene.DecorationSettings.SmallPopulationThreshold)
                {
                    SetCellDecoration(cell, data, "Small Settlement");
                }
                if(data.Decoration.Key != null)
                {
                    this.decorations[data.Decoration.Key].Update(updatedMeshes);
                }
            }

            //stopwatch.Stop();
            //Debug.WriteLine("Took " + stopwatch.ElapsedMilliseconds + " ms to update vertexdata for " + updatedCells + " cells");
            foreach(var mesh in updatedMeshes)
            {
                meshUpdateQueue.Enqueue(mesh);
            }
        }

        /// <summary>
        /// Returns the vertex color a cell should have.
        /// </summary>
        private Color GetCellColor(Cell cell)
        {
            Color color;
            switch(cellColorMode)
            {
                case CellColorMode.Elevation:
                    color = Color.Lerp(Color.Black, Color.White,
                        (float)cell.Elevation / renderedScene.Settings.ElevationMultiplier);
                    break;
                case CellColorMode.Wetness:
                    color = Color.Lerp(Color.Black, Color.White, 
                        MathHelper.Clamp((float)cell.Wetness / renderedScene.Settings.RainMultiplier, 0, 1));
                    break;
                case CellColorMode.Temperature:
                    if(cell.Temperature < -20) { color = Color.White; }
                    else if (cell.Temperature < 0) { color = Color.LightBlue; }
                    else if (cell.Temperature < 10) { color = Color.Blue; }
                    else if (cell.Temperature < 20) { color = Color.Green; }
                    else if (cell.Temperature < 30) { color = Color.Yellow; }
                    else if (cell.Temperature < 40) { color = Color.OrangeRed; }
                    else { color = Color.Red; }
                    break;
                default:
                    color = cell.Owner != null ? cell.Owner.Color : Color.DarkSlateGray;
                    break;
            }
            return color;
        }

        /// <summary>
        /// Sets the decoration for a specific cell
        /// </summary>
        /// <param name="cell">The cell to set for</param>
        /// <param name="cellMappedData">The cell's mapped data</param>
        /// <param name="mesh">The decoration mesh</param>
        private void SetCellDecoration(Cell cell, CellMappedData cellMappedData, string decoration)
        {
            if(cellMappedData.Decoration.Key == decoration)
            {
                return;
            }

            if(cellMappedData.Decoration.Key != null)
            {
                this.decorations[cellMappedData.Decoration.Key].FreeIndex(cellMappedData.Decoration.Value);
            }

            if(decoration != null && decoration.Length > 0)
            {

                var index = this.decorations[decoration].GetFreeIndex();
                if (index < 0)
                {
                    //Debug.WriteLine("Unable to get free index for decoration " + decoration);
                    return;
                }
                cellMappedData.Decoration = new KeyValuePair<string, int>(decoration, index);
                this.decorations[decoration].SetPosition(index, new Vector3(TopographyHelper.HexToWorld(cell.Coordinates), (float)cell.Elevation));
            }
            else
            {
                cellMappedData.Decoration = new KeyValuePair<string, int>(null, 0);
            }
        }
    }
}
