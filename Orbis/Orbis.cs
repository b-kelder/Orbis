using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.Rendering;
using System;
using System.Collections.Generic;

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
        Material hexMaterial;
        Material pyramidMaterial;

        Camera camera;

        List<RenderInstance> renderInstances;

        private float rotation;
        private float distance;
        private float angle;

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

        private Mesh CreateHexMesh()
        {
            var vertices = new Vector3[6];
            var uvs = new Vector2[6];
            var triangles = new ushort[12];

            float sideYPos = (float)Math.Sin(MathHelper.ToRadians(30));
            float sideXPos = (float)Math.Cos(MathHelper.ToRadians(30));

            vertices[0] = new Vector3(0, 1, 0);
            vertices[1] = new Vector3(sideXPos, sideYPos, 0);
            vertices[2] = new Vector3(sideXPos, -sideYPos, 0);
            vertices[3] = new Vector3(0, -1, 0);
            vertices[4] = new Vector3(-sideXPos, -sideYPos, 0);
            vertices[5] = new Vector3(-sideXPos, sideYPos, 0);

            for(int i = 0; i < vertices.Length; i++)
            {
                uvs[i] = (new Vector2(vertices[i].X, vertices[i].Y) + Vector2.One) * 0.5f;
                uvs[i].Y = 1 - uvs[i].Y;
            }

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            triangles[6] = 0;
            triangles[7] = 3;
            triangles[8] = 4;

            triangles[9] = 0;
            triangles[10] = 4;
            triangles[11] = 5;

            var mesh = new Mesh()
            {
                Vertices = vertices,
                UVs = uvs,
                Triangles = triangles,
            };
            return mesh;
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
            var hexMesh = CreateHexMesh();
            var pyramidMesh = CreatePyramidMesh();
            var hexCombiner = new MeshCombiner();
            var pyramidCombiner = new MeshCombiner();

            Random rand = new Random();

            int range = 300;
            for(int p = -range; p <= range; p++)
            {
                for(int q = -range; q <= range; q++)
                {
                    if(Math.Abs(q + p) > range)
                    {
                        continue;
                    }
                    Vector3 position = new Vector3(TopographyHelper.HexToWorld(new Point(p, q)), 0);
                    hexCombiner.Add(new MeshInstance
                    {
                        mesh = hexMesh,
                        matrix = Matrix.CreateTranslation(position),
                    });

                    if(rand.Next(40) <= 1)
                    {
                        // Spawn randomly scaled piramid
                        pyramidCombiner.Add(new MeshInstance
                        {
                            mesh = pyramidMesh,
                            matrix = Matrix.CreateScale((float)rand.NextDouble() * 0.4f + 0.8f) * Matrix.CreateTranslation(position),
                        });
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
                    material = hexMaterial,
                    matrix = Matrix.Identity,
                });
            }
            var combinedPyramids = pyramidCombiner.GetCombinedMeshes();
            foreach(var mesh in combinedPyramids)
            {
                renderInstances.Add(new RenderInstance()
                {
                    mesh = new RenderableMesh(graphics.GraphicsDevice, mesh),
                    material = pyramidMaterial,
                    matrix = Matrix.Identity,
                });
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hexMaterial = new Material(basicShader);
            pyramidMaterial = new Material(basicShader);

            // TODO: use this.Content to load your game content here
            using(var stream = TitleContainer.OpenStream("Content/hex_grass.png"))
            {
                hexMaterial.Texture = Texture2D.FromStream(this.GraphicsDevice, stream);
            }

            using(var stream = TitleContainer.OpenStream("Content/hex_brick.png"))
            {
                pyramidMaterial.Texture = Texture2D.FromStream(this.GraphicsDevice, stream);
            }

            Mesh trainMesh;
            Material trainMat = new Material(basicShader);
            using(var stream = TitleContainer.OpenStream("Content/train.png"))
            {
                trainMat.Texture = Texture2D.FromStream(this.GraphicsDevice, stream);
            }

            using(var stream = TitleContainer.OpenStream("Content/train.obj"))
            {
                trainMesh = ObjParser.FromStream(stream);
            }

            renderInstances.Add(new RenderInstance
            {
                mesh = new RenderableMesh(GraphicsDevice, trainMesh),
                material = trainMat,
                matrix = Matrix.Identity
            });

            LoadRenderInstances();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //DrawHex();
            //DrawPiramids();
            //DrawMesh(meshTest, piramidEffect, this.texturePiramid);

            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            Matrix viewMatrix = camera.CreateViewMatrix();
            Matrix projectionMatrix = camera.CreateProjectionMatrix(aspectRatio);

            // Create batches sorted by material?
            var materialBatches = new Dictionary<Material, List<RenderInstance>>();
            foreach(var instance in renderInstances)
            {
                //DrawInstance(instance);
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


            base.Draw(gameTime);
        }

        /*void DrawInstance(PreparedRenderInstance instance)
        {
            instance.effect.World = instance.matrix;
            instance.effect.View = camera.CreateViewMatrix();
            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            instance.effect.Projection = camera.CreateProjectionMatrix(aspectRatio);
            instance.effect.TextureEnabled = true;
            instance.effect.Texture = instance.texture;

            graphics.GraphicsDevice.Indices = instance.mesh.IndexBuffer;
            graphics.GraphicsDevice.SetVertexBuffer(instance.mesh.VertexBuffer);

            foreach(var pass in instance.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                    0,
                    0,
                    instance.mesh.IndexBuffer.IndexCount);
            }
        }*/
    }
}
