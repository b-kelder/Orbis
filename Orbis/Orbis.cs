using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.Engine;
using Orbis.World;
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

        List<Entity> entities = new List<Entity>();
        Scene scene;
        RenderTarget2D rt;
        bool kek = false;

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
            // TODO: Add your initialization logic here
            rt = new RenderTarget2D(GraphicsDevice, 1000, 1000);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            WorldGenerator worldGenerator = new WorldGenerator(20011998);

            scene = new Scene();
            worldGenerator.GenerateWorld(scene, 100, 100);

            // Call all drawables
            foreach (var item in entities)
            {
                item.LoadContent(Content);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Call all drawables
            foreach (var item in entities)
            {
                item.UnloadContent();
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Call all drawables
            foreach (var item in entities)
            {
                item.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (!kek)
            {
                var rectTexture = new Texture2D(GraphicsDevice, 1, 1);
                rectTexture.SetData(new[] { Color.White });
                
                spriteBatch.Begin();
                GraphicsDevice.SetRenderTarget(rt);
                for (int x = 0; x < scene.WorldMap.GetLength(0); x++)
                {
                    for (int y = 0; y < scene.WorldMap.GetLength(1); y++)
                    {
                        Cell cell = scene.WorldMap[x, y];

                        spriteBatch.Draw(rectTexture, new Rectangle(x * 10, y * 10, 10, 10), new Color((float)cell.NoiseValue, (float)cell.NoiseValue, (float)cell.NoiseValue));
                    }
                }

                kek = true;

                spriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);
                
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rt, new Rectangle(Point.Zero, new Point(1000, 1000)), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);

            // Call all drawables
            //foreach (var item in entities)
            //{
            //    item.Draw(spriteBatch);
            //}
        }

        /// <summary>
        /// Add an entity to the world.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        /// <summary>
        /// Remove an entity if it exists.
        /// </summary>
        /// <param name="entity">Entity to remove.</param>
        public void RemoveEntity(Entity entity)
        {
            if (entities.Contains(entity))
            {
                entities.Remove(entity);
            }
        }
    }
}
