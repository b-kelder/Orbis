using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     A basic element in the Orbis UI that can be drawn.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public interface IRenderableElement
    {
        /// <summary>
        ///     The combination of position and size for the element.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        ///     The layer depth of the element.
        /// </summary>
        /// 
        /// <remarks>
        ///     Value should be between 0 and 1, with zero being the front and 1 being the back.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException" />
        float LayerDepth { get; set; }

        /// <summary>
        ///     The position of the element.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        ///     Render the UI Element using the given spriteBatch.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spriteBatch to use for drawing;
        /// </param>
        void Render(SpriteBatch spriteBatch);
    }
}
