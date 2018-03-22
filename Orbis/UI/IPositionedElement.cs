using Microsoft.Xna.Framework;

namespace Orbis.UI
{
    /// <summary>
    ///     Interface for UI elements with an absolute on-screen position.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public interface IPositionedElement
    {
        /// <summary>
        ///     The combination of absolute position and size for the element.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        ///     The absolute position of the element.
        /// </summary>
        Point Position { get; }

        /// <summary>
        ///     The dimensions of the element.
        /// </summary>
        Point Size { get; }
    }
}
