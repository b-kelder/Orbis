using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     An element in the UI that has a position relative to its parent.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public abstract class RelativeElement : IPositionedElement
    {
        /// <summary>
        ///     The position within the parent relative to which the element is placed.
        /// </summary>
        public AnchorPosition AnchorPosition { get; set; }

        /// <summary>
        ///     The combination of absolute position and size of the element.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position, Size);
            }
        }

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
                        position.Y += parentBounds.Top;
                        break;
                    case AnchorPosition.Center:
                        position += parentBounds.Center;
                        break;
                    case AnchorPosition.BottomLeft:
                        position.X += parentBounds.Left;
                        position.Y += parentBounds.Bottom;
                        break;
                    case AnchorPosition.BottomRight:
                        position.X += parentBounds.Right;
                        position.Y += parentBounds.Bottom;
                        break;
                    case AnchorPosition.TopLeft:
                    default:
                        position.X += parentBounds.Left;
                        position.Y += parentBounds.Top;
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
        public virtual Point Size { get; set; }

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
