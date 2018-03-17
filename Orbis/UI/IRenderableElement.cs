using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbis.UI
{
    /// <summary>
    ///     Interface for UI elements that can be rendered.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public interface IRenderableElement
    {
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
        ///     Render the UI Element using the given spriteBatch.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spriteBatch to use for drawing;
        /// </param>
        void Render(SpriteBatch spriteBatch);
    }
}
