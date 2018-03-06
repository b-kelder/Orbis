using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Exceptions;

namespace Orbis.UI
{
    /// <summary>
    ///     Represents an element in the Orbis UI.
    /// </summary>
    public abstract class UIElement : IDrawable
    {
        /// <summary>
        ///     The position of the anchor for this element;
        /// </summary>
        public UIAnchorPosition AnchorPosition { get; set; }

        /// <summary>
        ///     The parent element for this UI Element.
        /// </summary>
        public UIElement Parent
        {
            get;
            // Only other UI Elements can set the parent, and shouldn't unless it is in the Addchild of the parent.
            protected set;
        }

        /// <summary>
        ///     The children of this UI Element.
        /// </summary>
        public UIElement[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }
        protected List<UIElement> _children;

        /// <summary>
        ///     The rectangle of the element relative to the parent element.
        /// </summary>
        public Rectangle RelativeRectangle
        {
            get
            {
                var relativeRect = new Rectangle(_elementRect.Location, _elementRect.Size);

                // If the element has a parent, the absolute position needs to e converted to a relative one.
                if (Parent != null)
                {
                    relativeRect.Location = relativeRect.Location - Parent._elementRect.Location;
                }
                return relativeRect;
            }
            set
            {
                var absoluteRect = value;

                // If the item has a parent, the relative position must be converted to an absolute position for drawing.
                if (Parent != null)
                {
                    var parentRect = Parent._elementRect;

                    //if (AnchorPosition == UIAnchorPosition.TopRight)
                    //{
                    //    absoluteRect.X = parentRect.Right - absoluteRect.X - absoluteRect.Width;
                    //}

                    if (absoluteRect.X > parentRect.Width || absoluteRect.X < 0
                            || absoluteRect.Y > parentRect.Y || absoluteRect.Y < 0)
                    {
                        throw new OrbisUIException("Relative position may not exceed the boundaries of the parent.");
                    }

                    if (absoluteRect.X + absoluteRect.Width > parentRect.Width
                        || absoluteRect.Y + absoluteRect.Height > parentRect.Height)
                    {
                        throw new OrbisUIException("Size may not exceed the boundaries of the parent.");
                    }

                    absoluteRect.Location = absoluteRect.Location + parentRect.Location;
                }

                _elementRect = absoluteRect;
            }
        }
        protected Rectangle _elementRect;

        /// <summary>
        ///     I don't even quite know what this does.
        /// </summary>
        public int DrawOrder
        {
            get;
            set;
        }

        /// <summary>
        ///     Is the element visible?
        /// </summary>
        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        ///     I don't know what this does.
        /// </summary>
        public event EventHandler<EventArgs> DrawOrderChanged;

        /// <summary>
        ///     I don't know what this does.
        /// </summary>
        public event EventHandler<EventArgs> VisibleChanged;

        /// <summary>
        ///     Base constructor for UI elements.
        /// </summary>
        public UIElement() {
            Parent = null;
            _elementRect = new Rectangle();
            _children = new List<UIElement>();
        }

        /// <summary>
        ///     Base constructor for UI elements.
        /// </summary>
        /// <param name="parent">
        ///     The parent element for this UI Element.
        /// </param>
        public UIElement(UIElement parent)
        {
            _elementRect = new Rectangle();
            _children = new List<UIElement>();
        }

        /// <summary>
        ///     Base constructor for UI elements.
        /// </summary>
        public UIElement(int x, int y, int width, int height)
        {
            _elementRect = new Rectangle(x, y, width, height);
            _children = new List<UIElement>();
        }

        /// <summary>
        ///     Base constructor for UI elements.
        /// </summary>
        /// <param name="parent">
        ///     The parent element for this UI Element.
        /// </param>
        public UIElement(int x, int y, int width, int height, UIElement parent)
        {
            _elementRect = new Rectangle(x, y, width, height);
            _children = new List<UIElement>();
        }

        /// <summary>
        ///     Draw the UI Element.
        /// </summary>
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public virtual void Draw(GameTime gameTime)
        {
            foreach (UIElement child in Children)
            {
                child.Draw(gameTime);
            }
        }

        /// <summary>
        ///     Add a child to the UI Element.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public void AddChild(UIElement child)
        {
            if (!_elementRect.Contains(child._elementRect.Location))
            {
                throw new OrbisUIException("Cannot add children with a position outside the boundaries of the parent.");
            }

            child._elementRect = Rectangle.Intersect(_elementRect, child._elementRect);

            _children.Add(child);
            child.Parent = this;
        }
    }
}
