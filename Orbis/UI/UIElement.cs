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
    public abstract class UIElement
    {
        /// <summary>
        ///     The parent element for this UI Element.
        /// </summary>
        public UIElement Parent
        {
            get;
            protected set;
        }

        /// <summary>
        ///     The children of this UI Element.
        /// </summary>
        abstract public UIElement[] Children
        {
            get;
        }

        /// <summary>
        ///     A rectangle with the absolute on-screen position and size of the element.
        /// </summary>
        public Rectangle AbsoluteRectangle
        {
            get
            {
                var absoluteRect = new Rectangle(_relativeRect.Location, _relativeRect.Size);

                // If the element has a parent, the absolute position needs to e converted to a relative one.
                if (Parent != null)
                {
                    absoluteRect.Location = absoluteRect.Location + Parent.AbsoluteRectangle.Location;
                }

                return absoluteRect;
            }
        }

        /// <summary>
        ///     The size of the UI Element.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public Point Size
        {
            get
            {
                return _relativeRect.Size;
            }
            set
            {
                // Negative sizes don't work for drawing.
                if (value.X < 0 || value.Y < 0)
                {
                    throw new OrbisUIException("Element size can not be negative.");
                }

                // To prevent issues, elements are not allowed exceed the boundaries of their parent.
                if (Parent != null)
                {
                    var parentRect = Parent.AbsoluteRectangle;
                    var newAbsoluteRect = new Rectangle(_relativeRect.Location + parentRect.Location, value);

                    CheckElementBoundaries(newAbsoluteRect, parentRect);
                }

                if (_relativeRect.Size != value)
                {
                    // Resetting the layout if the value hasn't changed is inefficiënt and therefore avoided.
                    _relativeRect.Size = value;
                    ResetLayout();
                }
                
            }
        }

        /// <summary>
        ///     The location of the element relative to its parent.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public Point RelativeLocation
        {
            get
            {
                return _relativeRect.Location;
            }
            set
            {
                if (Parent != null)
                {
                    var parentRect = Parent.AbsoluteRectangle;
                    var newAbsoluteRect = new Rectangle(value + parentRect.Location, _relativeRect.Size);

                    CheckElementBoundaries(newAbsoluteRect, parentRect);
                }

                if (_relativeRect.Location != value)
                {
                    // Resetting layout if the value has not changed is inefficiënt and therefore avoided.
                    _relativeRect.Location = value;
                    ResetLayout();
                }
            }
        }

        /// <summary>
        ///     The size of the element and location relative to the parent.
        /// </summary>
        public Rectangle RelativeRectangle
        {
            get
            {
                return _relativeRect;
            }
            set
            {
                if (Parent != null)
                {
                    // The rectangle is relative to the parent, so it should be calculated based on the parent.
                    var parentRect = Parent.AbsoluteRectangle;
                    var newAbsoluteRect = new Rectangle(value.Location + parentRect.Location, value.Size);

                    CheckElementBoundaries(newAbsoluteRect, parentRect);
                }

                if (_relativeRect != value)
                {
                    // Resetting layout if the value has not changed is inefficiënt and therefore avoided.
                    _relativeRect = value;
                    ResetLayout();
                }
            }
        }

        /// <summary>
        ///     Represents the relative position and size of the UI Element.
        /// </summary>
        protected Rectangle _relativeRect;

        /// <summary>
        ///     Base constructor for UI elements.
        /// </summary>
        public UIElement() {
            Parent = null;
            _relativeRect = Rectangle.Empty;
        }

        /// <summary>
        ///     Draw the UI Element.
        /// </summary>
        /// <param name="spriteBatch">
        ///     The SpriteBatch to use for drawing textures.
        /// </param>
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (UIElement child in Children)
            {
                child.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        ///     Reset the layout of the element and all of its children.
        /// </summary>
        public virtual void ResetLayout()
        {
            foreach (UIElement child in Children)
            {
                child.ResetLayout();
            }
        }

        /// <summary>
        ///     Add a child to the UI Element.
        /// </summary>
        /// 
        /// <param name="child">
        ///     The child to add.
        /// </param>
        /// 
        /// <exception cref="OrbisUIException" />
        public virtual void AddChild(UIElement child)
        {
            child.Parent = this;
            ResetLayout();
        }

        /// <summary>
        ///     Replace one of the element's children.
        /// </summary>
        /// 
        /// <param name="childIndex">
        ///     The index of the child to replace.
        /// </param>
        /// <param name="newChild">
        ///     The child to replace it with.
        /// </param>
        /// 
        /// <exception cref="OrbisUIException" />
        public virtual void ReplaceChild(int childIndex, UIElement newChild)
        {
            newChild.Parent = this;
            ResetLayout();
        }

        /// <summary>
        ///     Check if the given absolute rectangle fits within the given parent rectangle
        ///     and throw an appropriate exception if it doesn't.
        /// </summary>
        /// 
        ///<param name="absoluteRect">
        ///     The absolute rectangle to check.
        ///</param>
        ///<param name="parentRect">
        ///     The rect within which the absoluterect needs to exist.
        ///</param>
        ///
        ///<exception cref="OrbisUIException" />
        protected void CheckElementBoundaries(Rectangle absoluteRect, Rectangle parentRect)
        {
            if (parentRect.Width != 0 && parentRect.Height != 0)
            {
                if (!parentRect.Contains(absoluteRect.Location))
                {
                    throw new OrbisUIException("Element location can not be outside the parent boundaries.");
                }

                if (absoluteRect.Width > parentRect.Width
                    || absoluteRect.Height > parentRect.Height)
                {
                    throw new OrbisUIException("Element can not expand beyond the parent.");
                }
            }
        }
    }
}
