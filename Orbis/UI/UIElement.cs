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
        ///     The anchor mode for this element.
        ///     Decides where the element anchors to the parent.
        /// </summary>
        public AnchorPosition AnchorPosition
        {
            get
            {
                return _anchorPos;
            }
            set
            {
                // Resetting layout when the value hasn't changed is inefficiënt and therefore avoided.
                if (_anchorPos != value)
                {
                    _anchorPos = value;
                    UpdateLayout();
                }
            }
        }
        /// <summary>
        ///     The position in the parent element relative to which this element is positioned.
        /// </summary>
        protected AnchorPosition _anchorPos;

        /// <summary>
        ///     Is the UI Element visible?
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     A rectangle with the absolute on-screen position and size of the element.
        /// </summary>
        public Rectangle AbsoluteRectangle
        {
            get
            {
                return GetNewAbsoluteRect(_relativeRect);
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
                // Prevent pointless checks by only changing if the value is actually different.
                if (_relativeRect != value)
                {
                    if (Parent != null)
                    {
                        // The rectangle is relative to the parent, so it should be calculated based on the parent.
                        Rectangle newAbsoluteRect = GetNewAbsoluteRect(value);

                        CheckElementBoundaries(newAbsoluteRect, Parent.AbsoluteRectangle);
                    }

                    _relativeRect = value;
                    UpdateLayout();
                }
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
                // Prevent pointless checks by only changing if the value is actually different.
                if (_relativeRect.Size != value)
                {
                    // Negative sizes don't work for drawing.
                    if (value.X < 0 || value.Y < 0)
                    {
                        throw new OrbisUIException("Element size can not be negative.");
                    }

                    // To prevent issues, elements are not allowed exceed the boundaries of their parent.
                    if (Parent != null)
                    {
                        Rectangle parentRect = Parent.AbsoluteRectangle;
                        Rectangle newAbsoluteRect = GetNewAbsoluteRect(new Rectangle(_relativeRect.Location, value));

                        CheckElementBoundaries(newAbsoluteRect, parentRect);
                    }
                    
                    _relativeRect.Size = value;
                    UpdateLayout();
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
                // Prevent pointless checks by only changing when the value is different from the current one.
                if (_relativeRect.Location != value)
                {
                    if (Parent != null)
                    {
                        Rectangle parentRect = Parent.AbsoluteRectangle;
                        Rectangle newAbsoluteRect = GetNewAbsoluteRect(new Rectangle(value, Size));

                        CheckElementBoundaries(newAbsoluteRect, parentRect);
                    }

                    _relativeRect.Location = value;
                    UpdateLayout();
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
            _anchorPos = AnchorPosition.TopLeft;
            Visible = true;
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        /// 
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (UIElement child in Children)
            {
                child.Update(gameTime);
            }
        }

        /// <summary>
        ///     Draw the UI Element.
        /// </summary>
        /// 
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
        ///     Update the layout of the element and all of its children.
        /// </summary>
        public virtual void UpdateLayout()
        {
            foreach (UIElement child in Children)
            {
                child.UpdateLayout();
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

            UpdateLayout();

            CheckElementBoundaries(child.AbsoluteRectangle, this.AbsoluteRectangle);
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
            
            UpdateLayout();

            CheckElementBoundaries(newChild.AbsoluteRectangle, this.AbsoluteRectangle);
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

        /// <summary>
        ///     Get the absolute rectangle for a relative rectangle based on the parent.
        /// </summary>
        /// 
        /// <param name="relativeRect">
        ///     The relative rectangle.
        /// </param>
        /// 
        /// <returns>
        ///     The new absolute rectangle
        /// </returns>
        protected Rectangle GetNewAbsoluteRect(Rectangle relativeRect)
        {
            var absoluteRect = new Rectangle(relativeRect.Location, relativeRect.Size);

            // If the element has a parent, the absolute position needs to e converted to a relative one.
            if (Parent != null)
            {
                Rectangle parentRect = Parent.AbsoluteRectangle;

                // Anchor positions decide what point of the parent the element is relative to.
                if (_anchorPos == AnchorPosition.TopLeft)
                {
                    absoluteRect.Location = absoluteRect.Location + parentRect.Location;
                }
                else if (_anchorPos == AnchorPosition.TopRight)
                {
                    Point parentTopRight = new Point(parentRect.Right, parentRect.Top);
                    absoluteRect.Location = absoluteRect.Location + parentTopRight;
                }
                else if (_anchorPos == AnchorPosition.Center)
                {
                    absoluteRect.Location = absoluteRect.Location + parentRect.Center;
                }
                else if (_anchorPos == AnchorPosition.BottomLeft)
                {
                    absoluteRect.Y += parentRect.Bottom;
                }
                else if (_anchorPos == AnchorPosition.BottomRight)
                {
                    Point parentBottomRight = new Point(parentRect.Right, parentRect.Bottom);
                    absoluteRect.Location = absoluteRect.Location + parentBottomRight;
                }
            }

            return absoluteRect;
        }
    }
}
