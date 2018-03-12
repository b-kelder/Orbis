using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using Orbis.Engine;
using Orbis.Rendering;
using Orbis.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orbis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Orbis : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        BasicEffect basicShader;

        Camera camera;

        List<RenderInstance> renderInstances;

        private float rotation;
        private float distance;
        private float angle;
        private Rendering.Model hexModel;
        private Rendering.Model houseHexModel;
        private Rendering.Model waterHexModel;
        private Scene scene;

        private Task<List<RenderInstance>> meshTask;

        private SpriteFont fontDebug;

        float worldUpdateTimer;

        public Orbis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // Shaders
            basicShader = new BasicEffect(graphics.GraphicsDevice);

            // Camera stuff
            rotation = 0;
            distance = 20;
            angle = -60;

            camera = new Camera();
            //camera.Mode = CameraMode.Orthographic;

            renderInstances = new List<RenderInstance>();

            base.Initialize();
        }

        private List<RenderInstance> GenerateMeshesFromScene(Scene scene)
        {
            // Hex generation test
            var renderInstances = new List<RenderInstance>();
            var hexMesh = hexModel.Mesh;
            var houseHexMesh = houseHexModel.Mesh;
            var waterHexMesh = waterHexModel.Mesh;
            // Use mesh combiners to get a bit more performant mesh for now
            var hexCombiner = new MeshCombiner();
            var houseHexCombiner = new MeshCombiner();
            var waterHexCombiner = new MeshCombiner();

            // Create world meshes
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int range = scene.WorldMap.Radius;

            for(int p = -range; p <= range; p++)
            {
                for(int q = -range; q <= range; q++)
                {
                    var cell = scene.WorldMap.GetCell(p, q);
                    if(cell == null)
                    {
                        continue;
                    }

                    var worldPoint = TopographyHelper.HexToWorld(new Point(p, q));
                    var position = new Vector3(
                        worldPoint,
                        (float)cell.Elevation);
                    // Temporary way to make sea actually level
                    if(cell.IsWater)
                    {
                        position.Z = scene.WorldMap.SeaLevel;
                        waterHexCombiner.Add(new MeshInstance
                        {
                            mesh = waterHexMesh,
                            matrix = Matrix.CreateTranslation(position)
                        });
                    }
                    else
                    {
                        if(cell.Owner != null)
                        {
                            houseHexCombiner.Add(new MeshInstance
                            {
                                mesh = houseHexMesh,
                                matrix = Matrix.CreateTranslation(position)
                            });
                        }
                        else
                        {
                            hexCombiner.Add(new MeshInstance
                            {
                                mesh = hexMesh,
                                matrix = Matrix.CreateTranslation(position)
                            });
                        }
                    }
                }
            }

            // Combine meshes
            var combinedHexes = hexCombiner.GetCombinedMeshes();
            foreach(var mesh in combinedHexes)
            {
                renderInstances.Add(new RenderInstance()
                {
                    mesh = new RenderableMesh(graphics.GraphicsDevice, mesh),
                    material = hexModel.Material,
                    matrix = Matrix.Identity,
                });

                Debug.WriteLine("Adding hex mesh");
            }
            var combinedPyramids = houseHexCombiner.GetCombinedMeshes();
            foreach(var mesh in combinedPyramids)
            {
                renderInstances.Add(new RenderInstance()
                {
                    mesh = new RenderableMesh(graphics.GraphicsDevice, mesh),
                    material = houseHexModel.Material,
                    matrix = Matrix.Identity,
                });

                Debug.WriteLine("Adding civ home base mesh");
            }
            combinedPyramids = waterHexCombiner.GetCombinedMeshes();
            foreach(var mesh in combinedPyramids)
            {
                renderInstances.Add(new RenderInstance()
                {
                    mesh = new RenderableMesh(graphics.GraphicsDevice, mesh),
                    material = waterHexModel.Material,
                    matrix = Matrix.Identity,
                });

                Debug.WriteLine("Adding water mesh");
            }

            stopwatch.Stop();
            Debug.WriteLine("Generated meshes in " + stopwatch.ElapsedMilliseconds + " ms");
            return renderInstances;
        }

        private async void GenerateWorld(int seed)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // Generate world
            Debug.WriteLine("Generating world for seed " + seed);
            scene = new Scene();
            var generator = new WorldGenerator(seed);
            generator.GenerateWorld(scene, 100);
            generator.GenerateCivs(scene, 500);

            stopwatch.Stop();
            Debug.WriteLine("Generated world in " + stopwatch.ElapsedMilliseconds + " ms");

            // Await for a previous mesh generation to finish if it hasn't yet
            if(meshTask != null)
            {
                await meshTask;
            }
            meshTask = Task<List<RenderInstance>>.Run(() => {
                return GenerateMeshesFromScene(scene);
            });

            // Set cam to sea level
            camera.LookTarget = new Vector3(camera.LookTarget.X, camera.LookTarget.Y, generator.SeaLevel);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            // Config Test
            XMLModel.Civilization[] civData = Content.Load<XMLModel.Civilization[]>("Config/Main");
            Debug.WriteLine(civData[0].name);
            Debug.WriteLine(civData[0].foodModifier);

            Debug.WriteLine(civData[1].name);
            Debug.WriteLine(civData[1].foodModifier);
            // End Config Test

            hexModel = ModelLoader.LoadModel("Content/Meshes/hex.obj", "Content/Textures/hex.png",
                basicShader, GraphicsDevice);
            houseHexModel = ModelLoader.LoadModel("Content/Meshes/hex_house.obj", "Content/Textures/hex_house.png",
                basicShader, GraphicsDevice);
            waterHexModel = ModelLoader.LoadModel("Content/Meshes/hex.obj", "Content/Textures/hex_water.png",
                basicShader, GraphicsDevice);

            fontDebug = Content.Load<SpriteFont>("DebugFont");

            GenerateWorld(1499806334);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Check if new meshes have been generated by task
            if(meshTask != null && meshTask.Status == TaskStatus.RanToCompletion)
            {
                // ALWAYS dispose of RenderMesh objects that won't be used anymore to reclaim
                // the VertexBuffer and IndexBuffer memory they use
                // Currently none of them will be reused so we dispose all of them
                foreach(var instance in this.renderInstances)
                {
                    instance.mesh.Dispose();
                }
                this.renderInstances = meshTask.Result;
                meshTask = null;
            }

            // See if world must be regenerated (TEST)
            if(worldUpdateTimer > 5.0f)
            {
                // TODO: Actual threading inside WorldGenerator?
                Task.Run(() => GenerateWorld(new Random().Next()));
                worldUpdateTimer = 0;
            }
            worldUpdateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            var state = Keyboard.GetState();
            var camMoveDelta = Vector3.Zero;

            float speed = 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            float scale = camera.OrthographicScale;

            if(state.IsKeyDown(Keys.LeftShift))
            {
                speed /= 5;
            }

            if(state.IsKeyDown(Keys.Up))
            {
                angle -= speed;
            }
            if(state.IsKeyDown(Keys.Down))
            {
                angle += speed;
            }
            if(state.IsKeyDown(Keys.Left))
            {
                rotation -= speed;
            }
            if(state.IsKeyDown(Keys.Right))
            {
                rotation += speed;
            }
            if(state.IsKeyDown(Keys.OemPlus))
            {
                distance -= speed;
                //scale -= speed;
            }
            if(state.IsKeyDown(Keys.OemMinus))
            {
                distance += speed;
                //scale += speed;
            }

            if(state.IsKeyDown(Keys.W))
            {
                camMoveDelta.Y += speed * 0.07f;
            }
            if(state.IsKeyDown(Keys.A))
            {
                camMoveDelta.X -= speed * 0.07f;
            }
            if(state.IsKeyDown(Keys.S))
            {
                camMoveDelta.Y -= speed * 0.07f;
            }
            if(state.IsKeyDown(Keys.D))
            {
                camMoveDelta.X += speed * 0.07f;
            }

            angle = MathHelper.Clamp(angle, -80, -5);
            distance = MathHelper.Clamp(distance, 1, 4000);

            //camera.OrthographicScale = MathHelper.Clamp(scale, 0.1f, 1000f);

            camera.LookTarget = camera.LookTarget + Vector3.Transform(camMoveDelta, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)));

            var camMatrix = Matrix.CreateTranslation(0, -distance, 0) *
               Matrix.CreateRotationX(MathHelper.ToRadians(angle)) *
               Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            camera.Position = Vector3.Transform(Vector3.Zero, camMatrix) + camera.LookTarget;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aqua);

            // Required when using SpriteBatch as well
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            Matrix viewMatrix = camera.CreateViewMatrix();
            Matrix projectionMatrix = camera.CreateProjectionMatrix(aspectRatio);

            // Create batches sorted by material?
            var materialBatches = new Dictionary<Material, List<RenderInstance>>();
            foreach(var instance in renderInstances)
            {
                if(!materialBatches.ContainsKey(instance.material))
                {
                    materialBatches.Add(instance.material, new List<RenderInstance>());
                }
                materialBatches[instance.material].Add(instance);
            }

            // Draw batches
            foreach(var batch in materialBatches)
            {
                var effect = batch.Key.Effect;
                effect.View = viewMatrix;
                effect.Projection = projectionMatrix;
                effect.Texture = batch.Key.Texture;
                effect.TextureEnabled = true;

                foreach(var instance in batch.Value)
                {
                    effect.World = instance.matrix;

                    graphics.GraphicsDevice.Indices = instance.mesh.IndexBuffer;
                    graphics.GraphicsDevice.SetVertexBuffer(instance.mesh.VertexBuffer);
                    foreach(var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            0,
                            0,
                            instance.mesh.IndexBuffer.IndexCount);
                    }
                }
            }
            
            spriteBatch.Begin();
            spriteBatch.DrawString(fontDebug, "STRING DRAWING TEST", new Vector2(10, 10), Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
