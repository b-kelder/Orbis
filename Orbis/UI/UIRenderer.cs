using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.BasicElements;
using Orbis.UI.Utility;

namespace Orbis.UI
{
    /// <summary>
    ///     A UI Window responsible for drawing the GUI and managing UI Elements.
    /// </summary>
    public class UIRenderer : DrawableGameComponent, IPositionedElement
    {
        Scrollbar test;

        /// <summary>
        ///     Gets the size of the window.
        /// </summary>
        private Point WindowSize
        {
            get;
            set;
        }

        public Rectangle Bounds => Game.Window.ClientBounds;

        public Point Position => Point.Zero;

        public Point Size => Game.Window.ClientBounds.Size;

        /// <summary>
        ///     The spritebatch used to draw the UI.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        ///     Represents the currently shown window.
        /// </summary>
        /// <param name="game"></param>
        public UIRenderer(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            BasicTextureFactory.CreateInstance(Game.GraphicsDevice);
            UIContentManager.CreateInstance(Game.Services);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            test = new Scrollbar(this)
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(15, 200)
            };

            base.Initialize();
        }

        /// <summary>
        ///     Peform the UI update for this frame.
        /// </summary>
        /// 
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            test.Update();
        }

        /// <summary>
        ///     Draw the UI on screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            test.Render(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
