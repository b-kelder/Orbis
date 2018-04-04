using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Windows;
using Orbis.UI.Utility;

namespace Orbis.UI
{
    /// <summary>
    ///     The game component responsible for drawing the GUI and updating other UI elements.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public class UIManager : DrawableGameComponent
    {
        /// <summary>
        ///     The current window shown by the UI.
        /// </summary>
        public UIWindow CurrentWindow { get; set; }

        /// <summary>
        ///     The spritebatch used to draw the UI.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        ///     Represents the currently shown window.
        /// </summary>
        /// <param name="game"></param>
        public UIManager(Game game) : base(game)
        {
            
        }

        /// <summary>
        ///     Change the current window to a new one.
        /// </summary>
        /// <param name="nextWindow">
        ///     The window to show.
        /// </param>
        public void ChangeWindow(UIWindow nextWindow)
        {
            CurrentWindow = nextWindow;
        }

        public override void Initialize()
        {
            UIContentManager.CreateInstance(Game);

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
            if (CurrentWindow != null)
            {
                CurrentWindow.Update();
            }
            base.Update(gameTime);
        }

        /// <summary>
        ///     Draw the UI on screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (CurrentWindow != null)
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront);
                CurrentWindow.Draw(_spriteBatch);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
