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

        double ocean = 0.45;

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

            WorldGenerator worldGenerator = new WorldGenerator(2018);

            scene = new Scene();
            worldGenerator.GenerateWorld(scene, 1000, 1000);
            worldGenerator.GenerateCivs(scene, 12);

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
                Color color;
                rectTexture.SetData(new[] { Color.White });
                
                spriteBatch.Begin();
                GraphicsDevice.SetRenderTarget(rt);
                for (int x = 0; x < scene.WorldMap.GetLength(0); x++)
                {
                    for (int y = 0; y < scene.WorldMap.GetLength(1); y++)
                    {
                        Cell cell = scene.WorldMap[x, y];

                        if (cell.Owner != null)
                        {
                            color = Color.Red;
                        }
                        else if (cell.NoiseValue < ocean)
                        {
                            color = new Color(0, 0, (float)cell.NoiseValue);
                        }
                        else if (cell.NoiseValue < ocean + 0.01)
                        {
                            color = new Color(127, 127, 0);
                        }
                        else if (cell.NoiseValue > 0.55)
                        {
                            color = new Color((float)cell.NoiseValue, (float)cell.NoiseValue, (float)cell.NoiseValue);
                        }
                        else if (cell.NoiseValue > 0.52)
                        {
                            color = new Color((float)cell.NoiseValue / 2, (float)cell.NoiseValue / 2, (float)cell.NoiseValue / 2);
                        }
                        else
                        {
                            color = new Color(0, 1 -(float)cell.NoiseValue, 0);
                        }

                        spriteBatch.Draw(rectTexture, new Rectangle(x, y, 1, 1), color);
                    }
                }

                kek = true;

                spriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);
                
            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(rt, new Rectangle(Point.Zero, new Point(2000, 2000)), Color.White);
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
