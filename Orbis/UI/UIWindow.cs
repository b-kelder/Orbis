using Microsoft.Xna.Framework;

namespace Orbis.UI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UIWindow : IPositionedElement
    {
        // Used for getting window size and text input.
        private Game _game;

        /// <summary>
        ///     The screen bounds of the window.
        /// </summary>
        public Rectangle Bounds => _game.Window.ClientBounds;

        /// <summary>
        ///     The size of the window.
        /// </summary>
        public Point Size => _game.Window.ClientBounds.Size;

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
        }
    }
}
