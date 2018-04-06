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
    public class SceneRendererComponent : DrawableGameComponent
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
            public float DefaultDensity { get; set; }
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

        private Dictionary<string, DecorationData> decorations = new Dictionary<string, DecorationData>();
        private Dictionary<string, BiomeMappedData> biomeMappedData = new Dictionary<string, BiomeMappedData>();
        private Dictionary<Cell, CellMappedData> cellMappedData = new Dictionary<Cell, CellMappedData>();
        private List<RenderableMesh> cellMeshes = new List<RenderableMesh>();
        private List<RenderInstance> renderInstances = new List<RenderInstance>();
        public Scene renderedScene;

        private Effect basicShader;
        private AtlasModelLoader modelLoader;
        private DecorationManager decorationManager;
        private Random random;

        /// <summary>
        /// Mode used for setting cell hex vertex colors. Only gets applied when cells are updated.
        /// </summary>
        private CellColorMode cellColorMode;        

        // Camera related fields
        private Camera camera;
        private float rotation;
        private float distance;
        private float angle;
        private Cell currentCamCell;
        private RenderableMesh tileHighlightMesh;

        // Concurrency and single threaded queueing
        private Queue<RenderableMesh> meshUpdateQueue = new Queue<RenderableMesh>();
        private Queue<KeyValuePair<Cell, CellMappedData>> defaultDecorationQueue = new Queue<KeyValuePair<Cell, CellMappedData>>();
        private Task<MeshGenerationResult> meshTask;        

        // Texture atlas debugging
        private const bool atlasDebugEnabled = false;
        private RenderInstance atlasDebugInstance;

        // Fog
        private Color skyColor;
        private float fogDistance = 1;

        /// <summary>
        /// Color of the sky and fog
        /// </summary>
        public Color SkyColor {
            get { return skyColor; }
            set
            {
                skyColor = value;
                if(basicShader != null)
                {
                    basicShader.Parameters["FogColor"].SetValue(skyColor.ToVector4());
                }
            }
        }

        /// <summary>
        /// Distance at which the fog is 100% density
        /// </summary>
        public float FogDistance {
            get { return fogDistance; }
            set
            {
                if(value > 0)
                {
                    fogDistance = value;
                    if (basicShader != null)
                    {
                        basicShader.Parameters["FogEndDistance"].SetValue(fogDistance);
                    }
                    if(camera != null)
                    {
                        camera.ClipFar = fogDistance;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the maximum allowed density for default cell decorations.
        /// Lower values can improve performance. Range 0 to 1.
        /// </summary>
        public float DecorationDensityCap { get; set; }

        /// <summary>
        /// Returns true if the renderer is ready to accept simulation updates.
        /// </summary>
        public bool ReadyForUpdate { get {
                return renderedScene != null && 
                    cellMappedData.Count > 0 &&
                    cellMeshes.Count > 0 && 
                    meshUpdateQueue.Count == 0 &&
                    defaultDecorationQueue.Count == 0;
            } }

        /// <summary>
        /// Maximum time we can spend in Update updating meshes.
        /// </summary>
        public float MaxUpdateTime { get; set; }
        /// <summary>
        /// Amount of active render instances.
        /// </summary>
        public int RenderInstanceCount { get { return renderInstances.Count; } }
        /// <summary>
        /// The cell currently highlighted by the camera. Might be null.
        /// </summary>
        public Cell HighlightedCell { get { return currentCamCell; } }
        /// <summary>
        /// Render target used by this component.
        /// </summary>
        public RenderTarget2D RenderTarget { get; set; }

        public SceneRendererComponent(Orbis game) : base(game)
        {
            MaxUpdateTime = 3;
            DecorationDensityCap = 1.0f;
            orbis = game;
            cellColorMode = CellColorMode.OwnerColor;

            rotation = 0;
            distance = 20;
            angle = -60;
            camera = new Camera();

            FogDistance = 350;
            SkyColor = Color.DeepSkyBlue;
        }

        public override void Initialize()
        {
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
            basicShader.Parameters["FogColor"].SetValue(SkyColor.ToVector4());
            basicShader.Parameters["FogEndDistance"].SetValue(FogDistance);

            // Load models, decoration and biome data
            modelLoader = new AtlasModelLoader(2048, 2048, basicShader, Game.Content);
            var decorationData = orbis.Content.Load<XMLModel.DecorationCollection>("Config/Decorations");
            decorationManager = new DecorationManager(decorationData, modelLoader, GraphicsDevice);
            // Load decorations
            foreach(var data in decorationData.Decorations)
            {
                decorations.Add(data.Name, new DecorationData(decorationManager.GetDecorationMesh(data.Name), GraphicsDevice, 1));
            }
            var biomeData = orbis.Content.Load<XMLModel.BiomeCollection>("Config/Biomes");
            biomeMappedData = new Dictionary<string, BiomeMappedData>();
            // Load biomes
            foreach (var biome in biomeData.Biomes)
            {
                biomeMappedData.Add(biome.Name, new BiomeMappedData
                {
                    HexMesh = modelLoader.LoadModel(biome.HexModel.Name, biome.HexModel.Texture, biome.HexModel.ColorTexture).Mesh,
                    DefaultDecoration = biome.DefaultDecoration,
                    DefaultDensity = biome.DecorationDensity,
                });
            }

            // Load tile highlight mesh
            var model = modelLoader.LoadModel("flag", "flag");
            tileHighlightMesh = new RenderableMesh(GraphicsDevice, model.Mesh);

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
            // Not worth updating if we don't have a scene
            if (renderedScene == null)
            {
                return;
            }

            #region Mesh and data updating
            // Check if new meshes have been generated by task
            if (meshTask != null)
            {
                if(meshTask.Status == TaskStatus.Canceled)
                {
                    Debug.WriteLine("Mesh task was cancelled, shouldn't ever happen, call programmer.");
                    meshTask = null;
                }
                else if(meshTask.Status == TaskStatus.Faulted)
                {
                    Debug.WriteLine("Mesh task crashed due to unhandled exception, call programmer.");
                    meshTask = null;
                }
                else if(meshTask.Status == TaskStatus.RanToCompletion)
                {
                    // Just in case this isn't the first map generated we must remove all cell decorations and existing terrain hex meshes
                    // Terrain meshes must be disposed since they won't be used again
                    foreach(var cellMesh in this.cellMeshes)
                    {
                        cellMesh.Dispose();
                    }
                    // Cell decorations are in cellMappedData, so they will be overwritten. Their meshes will be reused so MUST NOT be disposed
                    // Update main data sets
                    var meshData = meshTask.Result;
                    this.cellMappedData = meshData.cellData;
                    this.cellMeshes = meshData.renderableMeshes;
                    // Set cell decorations, this can lock up the application for a while
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    defaultDecorationQueue = new Queue<KeyValuePair<Cell, CellMappedData>>();
                    foreach (var cell in this.cellMappedData)
                    {
                        defaultDecorationQueue.Enqueue(cell);
                    }
                    meshTask = null;

                    stopwatch.Stop();
                    Debug.WriteLine("Took " + stopwatch.Elapsed.TotalMilliseconds + "ms to set decorations");
                }
            }

            // Spread default decoration setting over several frames
            if(defaultDecorationQueue.Count > 0)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                do
                {
                    var cell = defaultDecorationQueue.Dequeue();
                    var biomeData = biomeMappedData[cell.Key.Biome.Name];
                    if (biomeData.DefaultDecoration != null && random.NextDouble() < MathHelper.Clamp(biomeData.DefaultDensity, 0, DecorationDensityCap))
                    {
                        SetCellDecoration(cell.Key, cell.Value, biomeData.DefaultDecoration);
                    }
                } while (defaultDecorationQueue.Count > 0 && stopwatch.Elapsed.TotalMilliseconds <= this.MaxUpdateTime);

                // Check which decorations need to be updated this frame
                var set = new HashSet<RenderableMesh>();
                foreach (var dec in this.decorations)
                {
                    dec.Value.Update(set);
                }
                foreach (var mesh in set)
                {
                    meshUpdateQueue.Enqueue(mesh);
                }
                stopwatch.Stop();
            }
            // Update some vertex buffers if we have to without impact framerate too much
            if(meshUpdateQueue.Count > 0)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                // Use do-while to ensure we update at least one mesh per frame
                do
                {
                    var mesh = meshUpdateQueue.Dequeue();
                    mesh.UpdateVertexBuffer();
                } while(meshUpdateQueue.Count > 0 && stopwatch.Elapsed.TotalMilliseconds <= this.MaxUpdateTime);

                stopwatch.Stop();
            }
            #endregion

            // Camera movement
            var camMoveDelta = Vector3.Zero;
            float movementSpeed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds * (distance / 10);
            float rotationSpeed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float zoomSpeed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            #region Cam Input
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
            }
            if (orbis.Input.IsKeyHeld(Keys.OemMinus))
            {
                distance += zoomSpeed;
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
            #endregion
            // Clamp to > -90, otherwise camera can do a flip
            angle = MathHelper.Clamp(angle, -89.9f, -5);
            distance = MathHelper.Clamp(distance, 1, 4000);

            camera.LookTarget = camera.LookTarget + Vector3.Transform(camMoveDelta, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)));
            // Update highlighted cell, snap Camera elevation to it if we can
            var camTile = TopographyHelper.RoundWorldToHex(new Vector2(camera.LookTarget.X, camera.LookTarget.Y));
            currentCamCell = renderedScene.WorldMap.GetCell(camTile.X, camTile.Y);
            if(currentCamCell != null)
            {
                var pos = camera.LookTarget;
                pos.Z = currentCamCell.IsWater ? renderedScene.Settings.SeaLevel : (float)currentCamCell.Elevation;
                camera.LookTarget = pos;

                for(int i = 0; i < tileHighlightMesh.VertexData.Length; i++)
                {
                    tileHighlightMesh.VertexData[i].Color = currentCamCell.Owner != null ?
                        currentCamCell.Owner.Color :
                        new Color(230, 232, 237);
                }
                tileHighlightMesh.UpdateVertexBuffer();

                // Draw some cell debug data
                orbis.DrawDebugLine("Current cell: " + currentCamCell.Coordinates);
                orbis.DrawDebugLine("Owner: " + (currentCamCell.Owner != null ? currentCamCell.Owner.Name : "Nobody"));
                orbis.DrawDebugLine("Biome: " + currentCamCell.Biome.Name);
                orbis.DrawDebugLine("Temperature: " + currentCamCell.Temperature.ToString("#.#"));
                orbis.DrawDebugLine("Elevation: " + ((currentCamCell.Elevation - renderedScene.Settings.SeaLevel) * 450).ToString("#.#"));
                orbis.DrawDebugLine("Population: " + currentCamCell.population);
                orbis.DrawDebugLine("Food: " + currentCamCell.food.ToString("#.#"));
            }

            // Move camera back from its target
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
            DrawToTarget(RenderTarget);
            base.Draw(gameTime);
        }

        private void DrawToTarget(RenderTarget2D renderTarget)
        {
            var graphics = orbis.Graphics;
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.Clear(SkyColor);

            // Required because SpriteBatch resets these
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            float aspectRatio = orbis.Graphics.PreferredBackBufferWidth / (float)orbis.Graphics.PreferredBackBufferHeight;
            if(renderTarget != null)
            {
                aspectRatio = renderTarget.Width / (float)renderTarget.Height;
            }
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
            // Draw tile highlighter if we are on the map
            if (currentCamCell != null)
            {
                var pos = camera.LookTarget;/*new Vector3(TopographyHelper.HexToWorld(currentCamCell.Coordinates),
                    camera.LookTarget.Z);*/
                meshBatches[tileHighlightMesh] = new List<RenderInstance>()
                {
                    new RenderInstance()
                    {
                        mesh = tileHighlightMesh,
                        matrix = Matrix.CreateTranslation(pos),
                    }
                };
            }
            // Atlas debugging quad
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
                    basicShader.Parameters["WorldView"].SetValue(instance.matrix * viewMatrix);
                    basicShader.Parameters["Projection"].SetValue(projectionMatrix);

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

            GraphicsDevice.SetRenderTarget(null);
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

            // Await a previous mesh generation to if it hasn't finished yet
            if (meshTask != null)
            {
                await meshTask;
            }
            meshTask = Task.Run(() => {
                return GenerateMeshesFromScene(scene);
            });

            // Default cam to sea level
            camera.LookTarget = new Vector3(camera.LookTarget.X, camera.LookTarget.Y, scene.Settings.SeaLevel);
        }

        /// <summary>
        /// Generates terrain meshes, cell and biome data from a scene. Can be put in a task.
        /// </summary>
        /// <param name="scene">The scene to generate from</param>
        /// <returns>Results</returns>
        private MeshGenerationResult GenerateMeshesFromScene(Scene scene)
        {
            // Results
            var renderableMeshes = new List<RenderableMesh>();
            var rawMeshes = new List<Mesh>();
            var cellData = new Dictionary<Cell, CellMappedData>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Create world meshes
            var hexCombiner = new MeshCombiner();
            int range = scene.WorldMap.Radius;

            for (int p = -range; p <= range; p++)
            {
                for (int q = -range; q <= range; q++)
                {
                    var cell = scene.WorldMap.GetCell(p, q);
                    // Due to the hexagonal shape we might be out of bounds
                    if(cell == null)
                    {
                        continue;
                    }

                    var worldPoint = TopographyHelper.HexToWorld(new Point(p, q));
                    var position = new Vector3(worldPoint, (float)cell.Elevation);
                    var color = GetCellColor(cell);
                    Mesh mesh = biomeMappedData[cell.Biome.Name].HexMesh;

                    // Ensure sea is actually level
                    if (cell.IsWater && cell.Elevation < scene.Settings.SeaLevel)
                    {
                        position.Z = scene.Settings.SeaLevel;
                    }

                    // Add it to the combiner, using the cell's location as a tag so we can later
                    // get the vertexes of this hex inside the larger mesh
                    int meshIndex = hexCombiner.Add(new MeshInstance
                    {
                        mesh = mesh,
                        matrix = Matrix.CreateTranslation(position),
                        tag = new Point(p, q),
                        color = color,
                        useColor = true,
                    });

                    // Register partial cell mapped data to be used later
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
        /// Simulator is responsible for detemening when a cell should have its 
        /// visuals updated.
        /// MUST NOT be put in a task.
        /// </summary>
        /// <param name="cells">Cells to update</param>
        public void UpdateScene(Cell[] cells)
        {
            var updatedMeshes = new HashSet<RenderableMesh>();
            foreach(var cell in cells)
            {
                if(cell == null) { continue; }
                var data = cellMappedData[cell];
                // This cell isn't in our mapped data, so it's probably not even part of our scene
                if(data == null) { continue; }
                var mesh = cellMeshes[data.MeshIndex];
                // Set hex mesh colors in case ownership was changed
                foreach(var i in data.VertexIndexes)
                {
                    mesh.VertexData[i].Color = GetCellColor(cell);
                }
                updatedMeshes.Add(mesh);

                // Update decoration based on population of the tile
                var prevDecoration = data.Decoration.Key;
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

                // Check if previous or new decoration meshes need updating due to changes we just made
                if(prevDecoration != null)
                {
                    this.decorations[data.Decoration.Key].Update(updatedMeshes);
                }
                if(data.Decoration.Key != null)
                {
                    this.decorations[data.Decoration.Key].Update(updatedMeshes);
                }
            }

            // Enqueue all changed decoration meshes for actual updating in Update()
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

            // Free old decoration
            if(cellMappedData.Decoration.Key != null)
            {
                this.decorations[cellMappedData.Decoration.Key].FreeIndex(cellMappedData.Decoration.Value);
            }

            if(decoration != null && decoration.Length > 0)
            {
                var index = this.decorations[decoration].GetFreeIndex();
                if (index < 0)
                {
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
