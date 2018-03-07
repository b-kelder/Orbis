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
        UIElement UIRootPanel;

        public Orbis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Orbis";
            //graphics.ToggleFullScreen();
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

            System.Diagnostics.Debug.WriteLine("testKek");
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
            GraphicsDevice.Clear(Color.DarkGreen);

            UIRootPanel.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void BuidUI(SpriteBatch sb)
        {
            var menuBackground = Content.Load<Texture2D>(@"UI\Placeholder");
            UIRootPanel = new SplitPanel()
            {
                RelativeRectangle = new Rectangle(Point.Zero, Window.ClientBounds.Size),
                SplitDirection = SplitDirection.Vertical,
                FixedChild = 1
            };
            
            UIRootPanel.AddChild(new Panel()
            {
                SpriteBatch = sb,
                BackgroundTexture = menuBackground
            });

            var rightChild = new SplitPanel()
            {
                Split = 200,
                SplitDirection = SplitDirection.Horizontal,
                FixedChild = 0
            };
            UIRootPanel.AddChild(rightChild);

            var redRect = new Texture2D(GraphicsDevice, 1, 1);
            redRect.SetData(new[] { Color.Red });
            rightChild.AddChild(new Panel()
            {
                SpriteBatch = sb,
                BackgroundTexture = redRect
            });
            var yellowRect = new Texture2D(GraphicsDevice, 1, 1);
            yellowRect.SetData(new[] { Color.Yellow });
            rightChild.AddChild(new Button());
        }
    }
}
