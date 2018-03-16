using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.BasicElements;
using Orbis.UI.Utility;

namespace Orbis.UI
{
    /// <summary>
    ///     A UI Window responsible for drawing the GUI and managing UI Elements.
    /// </summary>
    public class UIRenderer : DrawableGameComponent
    {
        private Texture2D _BackgroundTexture;

        /// <summary>
        ///     Should the background be shown?
        /// </summary>
        public bool ShowBackground { get; set; }

        /// <summary>
        ///     Gets the size of the window.
        /// </summary>
        private Point WindowSize
        {
            get;
            set;
        }

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
        }

        /// <summary>
        ///     Draw the UI on screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            
            if (ShowBackground)
            {
                _spriteBatch.Draw(_BackgroundTexture, Game.Window.ClientBounds, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
