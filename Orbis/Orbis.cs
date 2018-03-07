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

        VertexPositionTexture[] testHex;
        VertexPositionTexture[] testVerts;
        BasicEffect effect;
        Texture2D texture;
        Vector3 camPos = new Vector3(0, 40, 20);

        BasicEffect hexEffect;
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
            testVerts = new VertexPositionTexture[6];

            testVerts[0].Position = new Vector3(-20, -20, -5);
            testVerts[1].Position = new Vector3(-20, 20, -5);
            testVerts[2].Position = new Vector3(20, -20, -5);

            testVerts[3].Position = testVerts[1].Position;
            testVerts[4].Position = new Vector3(20, 20, -5);
            testVerts[5].Position = testVerts[2].Position;

            testVerts[0].TextureCoordinate = new Vector2(0, 0);
            testVerts[1].TextureCoordinate = new Vector2(0, 1);
            testVerts[2].TextureCoordinate = new Vector2(1, 0);

            testVerts[3].TextureCoordinate = testVerts[1].TextureCoordinate;
            testVerts[4].TextureCoordinate = new Vector2(1, 1);
            testVerts[5].TextureCoordinate = testVerts[2].TextureCoordinate;

            var hexes = new List<VertexPositionTexture>();
            float sideXPos = (float)Math.Cos(MathHelper.ToRadians(30));
            float sideYPos = (float)Math.Sin(MathHelper.ToRadians(30));

            float rowOffset = 1 + sideYPos;
            float columnOffset = sideXPos;

            hexes.AddRange(CreateHex(new Vector3(0, 0, 0)));
            hexes.AddRange(CreateHex(new Vector3(-columnOffset * 2, 0, 0)));
            hexes.AddRange(CreateHex(new Vector3(columnOffset * 2, 0, 0)));
            hexes.AddRange(CreateHex(new Vector3(-columnOffset, rowOffset, 0)));
            hexes.AddRange(CreateHex(new Vector3(columnOffset, rowOffset, 0)));
            hexes.AddRange(CreateHex(new Vector3(-columnOffset, -rowOffset, 0)));
            hexes.AddRange(CreateHex(new Vector3(columnOffset, -rowOffset, 0)));

            hexes.AddRange(CreateHex(new Vector3(0, 0, 7)));
            hexes.AddRange(CreateHex(new Vector3(-columnOffset * 2, 0, 1)));
            hexes.AddRange(CreateHex(new Vector3(columnOffset * 2, 0, 2)));
            hexes.AddRange(CreateHex(new Vector3(-columnOffset, rowOffset, 3)));
            hexes.AddRange(CreateHex(new Vector3(columnOffset, rowOffset, 4)));
            hexes.AddRange(CreateHex(new Vector3(-columnOffset, -rowOffset, 5)));
            hexes.AddRange(CreateHex(new Vector3(columnOffset, -rowOffset, 6)));

            testHex = hexes.ToArray();

            // TODO: Add your initialization logic here
            effect = new BasicEffect(graphics.GraphicsDevice);
            hexEffect = new BasicEffect(graphics.GraphicsDevice);

            rotation = 0;
            distance = 20;
            angle = -45;

            base.Initialize();
        }

        public VertexPositionTexture[] CreateHex(Vector3 center)
        {
            var verts = new VertexPositionTexture[12];
            var uvMatrix = Matrix.CreateTranslation(new Vector3(0.5f, 0.5f, 0)) * Matrix.CreateScale(0.5f);

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
                var v3 = Vector3.Transform(verts[i].Position, uvMatrix);
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
            using(var stream = TitleContainer.OpenStream("Content/checkerboard.png"))
            {
                texture = Texture2D.FromStream(this.GraphicsDevice, stream);
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

            angle = MathHelper.Clamp(angle, -80, -5);

            var camMatrix = Matrix.CreateTranslation(0, -distance, 0) *
               Matrix.CreateRotationX(MathHelper.ToRadians(angle)) *
               Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));

            camPos = Vector3.Transform(Vector3.Zero, camMatrix);

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
            DrawTestVerts();
          

            base.Draw(gameTime);
        }

        void DrawTestVerts()
        {
            var camLookAtVector = Vector3.Zero;
            var camUpVector = Vector3.UnitZ;

            effect.View = Matrix.CreateLookAt(camPos, camLookAtVector, camUpVector);
            float aspectRatio = graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            float fov = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClipPlane, farClipPlane);

            effect.TextureEnabled = true;
            effect.Texture = texture;

            foreach(var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    testVerts,
                    0,
                    2);
            }
        }

        void DrawHex()
        {
            var camLookAtVector = Vector3.Zero;
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
    }
}
