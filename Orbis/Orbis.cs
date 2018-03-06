using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Orbis.UI;

namespace Orbis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Orbis : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        UIPanel UIRootPanel;

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

            // build the UI;
            BuidUI(spriteBatch);
            // TODO: use this.Content to load your game content here
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
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            UIRootPanel.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void BuidUI(SpriteBatch sb)
        {
            var menuBackground = Content.Load<Texture2D>(@"UI\Placeholder");
            UIRootPanel = new UIPanel(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, spriteBatch, menuBackground);
            var redRect = new Texture2D(GraphicsDevice, 1, 1);
            redRect.SetData(new[] { Color.Red });
            UIRootPanel.AddChild(new UIPanel(0, 0, 200, GraphicsDevice.Viewport.Height, spriteBatch, redRect));
            UIRootPanel.AddChild(new UIPanel(GraphicsDevice.Viewport.Width - 200, 0, 200, GraphicsDevice.Viewport.Height, spriteBatch, redRect));
        }
    }
}
