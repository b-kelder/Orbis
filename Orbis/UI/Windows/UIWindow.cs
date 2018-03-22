using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI.Windows
{
    /// <summary>
    ///     Abstract implementation of a window in the Orbis UI.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public abstract class UIWindow : IPositionedElement, IUpdatableElement
    {
        // Used for getting window size and text input.
        protected Game _game;

        /// <summary>
        ///     The screen bounds of the window.
        /// </summary>
        public Rectangle Bounds => _game.Window.ClientBounds; // Windows have the same bounds as the game window.

        /// <summary>
        ///     The size of the window.
        /// </summary>
        public Point Size => _game.Window.ClientBounds.Size; // Windows are the same size as the game window.

        /// <summary>
        ///     The position of the window.
        /// </summary>
        public Point Position => Point.Zero; // Windows always have the position (0,0).

        /// <summary>
        ///     Create a new <see cref="UIWindow"/>.
        /// </summary>
        /// <param name="game"></param>
        public UIWindow(Game game)
        {
            _game = game;
            _game.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        /// <summary>
        ///     Draw the UIWindow to the screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use for drawing.</param>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        ///     Event handler for resizing the window.
        /// </summary>
        protected abstract void Window_ClientSizeChanged(object sender, System.EventArgs e);

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public abstract void Update();
    }
}
