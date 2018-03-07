using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        VertexPositionTexture[] testPiramids;
        VertexPositionTexture[] testHex;
        Texture2D texture;
        Texture2D texturePiramid;
        Vector3 camPos = new Vector3(0, 40, 20);
        Vector3 camCenter = Vector3.Zero;

        BasicEffect hexEffect;
        BasicEffect piramidEffect;

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
            // Hex generation test
            var piramids = new List<VertexPositionTexture>();
            var hexes = new List<VertexPositionTexture>();
            Random rand = new Random();
            float sideXPos = (float)Math.Cos(MathHelper.ToRadians(30));
            float sideYPos = (float)Math.Sin(MathHelper.ToRadians(30));
            int range = 10;
            for(int p = -range; p <= range; p++)
            {
                for(int q = -range; q <= range; q++)
                {
                    if(Math.Abs(q + p) > range)
                    {
                        continue;
                    }
                    Vector2 position = TopographyHelper.HexToWorld(new Point(p, q));
                    hexes.AddRange(CreateHex(new Vector3(position.X, position.Y, 0)));

                    if(rand.Next(10) <= 1)
                    {
                        piramids.AddRange(CreatePiramid(new Vector3(position.X, position.Y, 0)));
                    }
                }
            }

            testHex = hexes.ToArray();
            testPiramids = piramids.ToArray();

            // TODO: Add your initialization logic here
            hexEffect = new BasicEffect(graphics.GraphicsDevice);
            piramidEffect = new BasicEffect(graphics.GraphicsDevice);

            rotation = 0;
            distance = 20;
            angle = -60;

            base.Initialize();
        }

        public VertexPositionTexture[] CreatePiramid(Vector3 center)
        {
            var verts = new VertexPositionTexture[12];
            verts[0].Position = new Vector3(-1, -1, 0);
            verts[1].Position = new Vector3(-1, 1, 0);
            verts[2].Position = new Vector3(0, 0, 2);

            verts[3].Position = verts[1].Position;
            verts[4].Position = new Vector3(1, 1, 0);
            verts[5].Position = verts[2].Position;

            verts[6].Position = verts[4].Position;
            verts[7].Position = new Vector3(1, -1, 0);
            verts[8].Position = verts[2].Position;

            verts[9].Position = verts[7].Position;
            verts[10].Position = verts[0].Position;
            verts[11].Position = verts[2].Position;

            verts[0].TextureCoordinate = new Vector2(1, 1);
            verts[1].TextureCoordinate = new Vector2(0, 1);
            verts[2].TextureCoordinate = new Vector2(0.5f, 0);

            verts[3].TextureCoordinate = verts[0].TextureCoordinate;
            verts[4].TextureCoordinate = verts[1].TextureCoordinate;
            verts[5].TextureCoordinate = verts[2].TextureCoordinate;

            verts[6].TextureCoordinate = verts[0].TextureCoordinate;
            verts[7].TextureCoordinate = verts[1].TextureCoordinate;
            verts[8].TextureCoordinate = verts[2].TextureCoordinate;

            verts[9].TextureCoordinate = verts[0].TextureCoordinate;
            verts[10].TextureCoordinate = verts[1].TextureCoordinate;
            verts[11].TextureCoordinate = verts[2].TextureCoordinate;

            for(int i = 0; i< verts.Length; i++)
            {
                verts[i].Position = verts[i].Position * 0.4f + center;
            }

            return verts;
        }

        public VertexPositionTexture[] CreateHex(Vector3 center)
        {
            var verts = new VertexPositionTexture[12];
            var uvMatrix = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(new Vector3(0.5f, 0.5f, 0)) * Matrix.CreateScale(1, -1, 1);

            float sideYPos = (float)Math.Sin(MathHelper.ToRadians(30));
            float sideXPos = (float)Math.Cos(MathHelper.ToRadians(30));

            verts[0].Position = center + new Vector3(0, 1, 0);
            verts[1].Position = center + new Vector3(sideXPos, sideYPos, 0);
            verts[2].Position = center + new Vector3(sideXPos, -sideYPos, 0);

            verts[3].Position = verts[0].Position;
            verts[4].Position = verts[2].Position;
            verts[5].Position = center + new Vector3(0, -1, 0);

            verts[6].Position = verts[5].Position;
            verts[7].Position = center + new Vector3(-sideXPos, -sideYPos, 0);
            verts[8].Position = center + new Vector3(-sideXPos, sideYPos, 0);

            verts[9].Position = verts[5].Position;
            verts[10].Position = verts[8].Position;
            verts[11].Position = verts[0].Position;

            for(int i = 0; i < verts.Length; i++)
            {
                var v3 = Vector3.Transform(verts[i].Position - center, uvMatrix);
                verts[i].TextureCoordinate = new Vector2(v3.X, v3.Y);
            }

            return verts;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            using(var stream = TitleContainer.OpenStream("Content/hex_grass.png"))
            {
                texture = Texture2D.FromStream(this.GraphicsDevice, stream);
            }

            using(var stream = TitleContainer.OpenStream("Content/hex_brick.png"))
            {
                texturePiramid = Texture2D.FromStream(this.GraphicsDevice, stream);
            }
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

            float speed = 20 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(state.IsKeyDown(Keys.LeftShift))
            {
                speed *= 5;
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
            }
            if(state.IsKeyDown(Keys.OemMinus))
            {
                distance += speed;
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

            camCenter += Vector3.Transform(camMoveDelta, Matrix.CreateRotationZ(MathHelper.ToRadians(rotation)));

            var camMatrix = Matrix.CreateTranslation(0, -distance, 0) *
               Matrix.CreateRotationX(MathHelper.ToRadians(angle)) *
               Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            camPos = Vector3.Transform(Vector3.Zero, camMatrix) + camCenter;

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
            DrawHex();
            DrawPiramids();

            base.Draw(gameTime);
        }

        void DrawHex()
        {
            var camLookAtVector = camCenter;
            var camUpVector = Vector3.UnitZ;

            hexEffect.View = Matrix.CreateLookAt(camPos, camLookAtVector, camUpVector);
            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fov = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            hexEffect.Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClipPlane, farClipPlane);

            hexEffect.TextureEnabled = true;
            hexEffect.Texture = texture;

            foreach(var pass in hexEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    testHex,
                    0,
                    testHex.Length / 3);
            }
        }

        void DrawPiramids()
        {
            var camLookAtVector = camCenter;
            var camUpVector = Vector3.UnitZ;

            piramidEffect.View = Matrix.CreateLookAt(camPos, camLookAtVector, camUpVector);
            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fov = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            piramidEffect.Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClipPlane, farClipPlane);

            piramidEffect.TextureEnabled = true;
            piramidEffect.Texture = texturePiramid;

            foreach(var pass in piramidEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    testPiramids,
                    0,
                    testPiramids.Length / 3);
            }
        }
    }
}
