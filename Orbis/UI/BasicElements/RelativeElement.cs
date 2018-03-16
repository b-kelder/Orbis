using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI.BasicElements
{
    public abstract class RelativeElement : IPositionedElement
    {
        /// <summary>
        ///     The position within the parent relative to which the element is placed.
        /// </summary>
        public AnchorPosition AnchorPosition { get; set; }

        /// <summary>
        ///     The combination of absolute position and size of the element.
        /// </summary>
        public abstract Rectangle Bounds { get; }

        /// <summary>
        ///     The parent element relative to which the element is placed.
        /// </summary>
        public IPositionedElement ParentElement { get; }

        /// <summary>
        ///     Get the absolute position of the element.
        /// </summary>
        public Point Position {
            get
            {
                Rectangle parentBounds = ParentElement.Bounds;
                Point position = RelativePosition;

                // The RelativePosition needs to be converted to an absolute position based on the anchor and parent bounds.
                switch (AnchorPosition)
                {
                    case AnchorPosition.TopRight:
                        position.X += parentBounds.Right;
                        break;
                    case AnchorPosition.Center:
                        position += parentBounds.Center;
                        break;
                    case AnchorPosition.BottomLeft:
                        position.Y += parentBounds.Bottom;
                        break;
                    case AnchorPosition.BottomRight:
                        position.X += parentBounds.Right;
                        position.Y += parentBounds.Bottom;
                        break;

                    case AnchorPosition.TopLeft:
                    default:
                        // The top left requires no changes.
                        break;
                }

                return position;
            }
        }

        /// <summary>
        ///     The relative position of the element.
        /// </summary>
        public Point RelativePosition { get; set; }

        /// <summary>
        ///     The dimensions of the element.
        /// </summary>
        public abstract Point Size { get; }

        /// <summary>
        ///     Create a new <see cref="RelativeElement"/>.
        /// </summary>
        /// 
        /// <param name="parent">
        ///     The element's parent.
        /// </param>
        public RelativeElement(IPositionedElement parent)
        {
            ParentElement = parent;
            RelativePosition = Point.Zero;
        }
    }
}
