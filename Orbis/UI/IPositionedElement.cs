using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.UI
{
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
