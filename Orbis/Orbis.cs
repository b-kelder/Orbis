using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI;
using Microsoft.Xna.Framework.Input;
using Orbis.Engine;
using Orbis.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Orbis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Orbis : Game
    {
        GraphicsDeviceManager graphics;
        UIWindow UI;


        BasicEffect basicShader;

        Camera camera;

        List<RenderInstance> renderInstances;

        Dictionary<string, ModelMesh> Meshes;

        List<Tuple<Matrix, ModelMesh>> WorldModels;

        private float rotation;
        private float distance;
        private float angle;
        private Rendering.Model hexModel;
        private Rendering.Model houseHexModel;


        public Orbis()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900
            };

            Content.RootDirectory = "Content";
            Window.Title = "Orbis";

            Meshes = new Dictionary<string, Microsoft.Xna.Framework.Graphics.ModelMesh>();
            WorldModels = new List<Tuple<Matrix, ModelMesh>>();

            UI = new UIWindow(this);
            Components.Add(UI);
            UI.DrawOrder = 1;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Window.AllowUserResizing = false;
            // TODO: Add your initialization logic here

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

        private Mesh CreatePyramidMesh()
        {
            var vertices = new Vector3[5];
            var uvs = new Vector2[5];
            var triangles = new ushort[12];

            vertices[0] = new Vector3(-0.5f, 0.5f, 0);
            vertices[1] = new Vector3(0.5f, 0.5f, 0);
            vertices[2] = new Vector3(0.5f, -0.5f, 0);
            vertices[3] = new Vector3(-0.5f, -0.5f, 0);
            vertices[4] = new Vector3(0, 0, 0.65f);

            uvs[0] = new Vector2(1, 1);
            uvs[1] = new Vector2(0, 1);
            uvs[2] = new Vector2(1, 1);
            uvs[3] = new Vector2(0, 1);
            uvs[4] = new Vector2(0.5f, 0);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 4;

            triangles[3] = 1;
            triangles[4] = 2;
            triangles[5] = 4;

            triangles[6] = 2;
            triangles[7] = 3;
            triangles[8] = 4;

            triangles[9] = 3;
            triangles[10] = 0;
            triangles[11] = 4;

            return new Mesh
            {
                Vertices = vertices,
                UVs = uvs,
                Triangles = triangles,
            };
        }

        private void LoadRenderInstances()
        {
            // Hex generation test
            //var hexMesh = hexModel.Mesh;
            //var houseHexMesh = houseHexModel.Mesh;
            //var hexCombiner = new MeshCombiner();
            //var houseHexCombiner = new MeshCombiner();

            var hexMesh = Meshes["hex"];
            var houseHexMesh = Meshes["houseHex"];
            List<ModelBone> modelBones = new List<ModelBone>();
            List<ModelMesh> modelMeshes = new List<ModelMesh>();
            

            Random rand = new Random();
            int range = 100;
            float amplitude = 25;

            var perlin = new Perlin(range);

            float boundsX = TopographyHelper.HexToWorld(new Point(range, 0)).X;
            float boundsY = TopographyHelper.HexToWorld(new Point(0, range)).Y;

            //Debug.WriteLine("HalfBounds: " + boundsX + " - " + boundsY);

            for(int p = -range; p <= range; p++)
            {
                for(int q = -range; q <= range; q++)
                {
                    if(Math.Abs(q + p) > range)
                    {
                        continue;
                    }

                    var vector = TopographyHelper.HexToWorld(new Point(p, q));
                    var perlinPoint = (vector + new Vector2(boundsX, boundsY))/ range;
                    var height = perlin.OctavePerlin(perlinPoint.X, perlinPoint.Y, 0, 4, 0.9);

                    Vector3 position = new Vector3(vector, (float)height * amplitude);

                    var translationMatrix = Matrix.CreateTranslation(position);

                    if (rand.Next(40) <= 1)
                    {
                        WorldModels.Add(new Tuple<Matrix, ModelMesh>(translationMatrix, houseHexMesh));

                        //houseHexCombiner.Add(new MeshInstance
                        //{
                        //    mesh = houseHexMesh,
                        //    matrix = Matrix.CreateTranslation(position),
                        //});
                    }
                    else
                    {
                        WorldModels.Add(new Tuple<Matrix, ModelMesh>(translationMatrix, hexMesh));
                        //hexCombiner.Add(new MeshInstance
                        //{
                        //    mesh = hexMesh,
                        //    matrix = Matrix.CreateTranslation(position),
                        //});
                    }
                }
            }

            // Combine meshes
            //var combinedHexes = hexCombiner.GetCombinedMeshes();
            //foreach(var mesh in combinedHexes)
            //{
            //    renderInstances.Add(new RenderInstance()
            //    {
            //        mesh = new RenderableMesh(graphics.GraphicsDevice, mesh),
            //        material = hexModel.Material,
            //        matrix = Matrix.Identity,
            //    });

            //    Debug.WriteLine("Adding hex mesh");
            //}
            //var combinedPyramids = houseHexCombiner.GetCombinedMeshes();
            //foreach(var mesh in combinedPyramids)
            //{
            //    renderInstances.Add(new RenderInstance()
            //    {
            //        mesh = new RenderableMesh(graphics.GraphicsDevice, mesh),
            //        material = houseHexModel.Material,
            //        matrix = Matrix.Identity,
            //    });

            //    Debug.WriteLine("Adding pyramid mesh");
            //}
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //hexModel = ModelLoader.LoadModel("Content/Meshes/hex.obj", "Content/Textures/hex.png",
            //    basicShader, GraphicsDevice);
            //houseHexModel = ModelLoader.LoadModel("Content/Meshes/hex_house.obj", "Content/Textures/hex_house.png",
            //    basicShader, GraphicsDevice);

            var hexModel = Content.Load<Microsoft.Xna.Framework.Graphics.Model>("Meshes/hex");
            var hexMesh = hexModel.Meshes[0];
            var hexTexture = Content.Load<Texture2D>("Textures\\hex");
            var hexEffect = hexMesh.Effects[0] as BasicEffect;
            hexEffect.Texture = hexTexture;
            hexEffect.TextureEnabled = true;
            Meshes.Add("hex", hexMesh);

            var houseHexModel = Content.Load<Microsoft.Xna.Framework.Graphics.Model>("Meshes/hex_house");
            var houseHexMesh = houseHexModel.Meshes[0];
            var houseHexTexture = Content.Load<Texture2D>("Textures/hex_house");
            var houseHexEffect = houseHexMesh.Effects[0] as BasicEffect;
            houseHexEffect.Texture = houseHexTexture;
            houseHexEffect.TextureEnabled = true;
            Meshes.Add("houseHex", houseHexMesh);

            LoadRenderInstances();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

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
            GraphicsDevice.SetRenderTarget(UI.SimulationWindow.SimulationRenderTarget);
            //GraphicsDevice.Clear(Color.DarkGreen);
            GraphicsDevice.Clear(Color.Aqua);

            // TODO: Add your drawing code here
            //DrawHex();
            //DrawPiramids();
            //DrawMesh(meshTest, piramidEffect, this.texturePiramid);

            float aspectRatio = UI.SimulationWindow.Size.X / (float)UI.SimulationWindow.Size.Y;
            //float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            Matrix viewMatrix = camera.CreateViewMatrix();
            Matrix projectionMatrix = camera.CreateProjectionMatrix(aspectRatio);

            //// Create batches sorted by material?
            //var materialBatches = new Dictionary<Material, List<RenderInstance>>();
            //foreach(var instance in renderInstances)
            //{
            //    //DrawInstance(instance);
            //    if(!materialBatches.ContainsKey(instance.material))
            //    {
            //        materialBatches.Add(instance.material, new List<RenderInstance>());
            //    }
            //    materialBatches[instance.material].Add(instance);
            //}

            foreach (var worldModel in WorldModels)
            {
                var effect = worldModel.Item2.Effects[0] as BasicEffect;
                effect.View = viewMatrix;
                effect.Projection = projectionMatrix;
                effect.TextureEnabled = true;
                effect.World = worldModel.Item1;

                worldModel.Item2.Draw();
            }

            //// Draw batches
            //foreach(var batch in materialBatches)
            //{
            //    var effect = batch.Key.Effect;
            //    effect.View = viewMatrix;
            //    effect.Projection = projectionMatrix;
            //    effect.Texture = batch.Key.Texture;
            //    effect.TextureEnabled = true;

            //    foreach(var instance in batch.Value)
            //    {
            //        effect.World = instance.matrix;

            //        graphics.GraphicsDevice.Indices = instance.mesh.IndexBuffer;
            //        graphics.GraphicsDevice.SetVertexBuffer(instance.mesh.VertexBuffer);
            //        foreach(var pass in effect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();

            //            graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
            //                0,
            //                0,
            //                instance.mesh.IndexBuffer.IndexCount);
            //        }
            //    }
            //}

            GraphicsDevice.SetRenderTarget(null);

            base.Draw(gameTime);
        }
    }
}
