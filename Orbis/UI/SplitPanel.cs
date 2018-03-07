using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Orbis.UI.Exceptions;

namespace Orbis.UI
{
    /// <summary>
    ///     A split panel layout that divides children in its boundaries.
    /// </summary>
    public class SplitPanel : UIElement
    {
        /// <summary>
        ///     The children of this split panel.
        /// </summary>
        public override UIElement[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }
        private List<UIElement> _children;

        /// <summary>
        ///     The position at which the divider between the two elements is placed.
        /// </summary>
        public int Split
        {
            get
            {
                return _split;
            }
            set
            {
                if (_split != value)
                {
                    _split = value;
                    ResetLayout();
                }
            }
        }
        private int _split;

        /// <summary>
        ///     Number of the child that has a fixed size.
        /// </summary>
        public int FixedChild
        {
            get
            {
                return _fixedChild;
            }
            set
            {
                if (value != 0 && value != 1)
                {
                    throw new OrbisUIException("Split panel fixed child index out of range.");
                }

                if (_fixedChild != value)
                {
                    _fixedChild = value;
                    ResetLayout();
                }
            }
        }
        private int _fixedChild;

        /// <summary>
        ///     The direction in which the panel splits the children.
        /// </summary>
        public SplitDirection SplitDirection
        {
            get
            {
                return _splitDirection;
            }
            set
            {
                if (_splitDirection != value)
                {
                    _splitDirection = value;
                    ResetLayout();
                }
            }
        }
        private SplitDirection _splitDirection;

        
        /// <summary>
        ///     Create a new <see cref="SplitPanel"/>.
        /// </summary>
        public SplitPanel() : base() {
            _children = new List<UIElement>(2);
            _split = 800;
            _splitDirection = SplitDirection.Vertical;
            _fixedChild = 1;
        }

        public override void Draw(GameTime gameTime)
        {
            // If drawing is attempted when the split panel ha's not been completed yet, an exception is thrown.
            // This should never happen intentionally.
            if (_children.Count < 2)
            {
                throw new OrbisUIException("Split panel is missing its children.");
            }

            base.Draw(gameTime);
        }

        /// <summary>
        ///     Reset the layout of the <see cref="SplitPanel"/> and all its children.
        /// </summary>
        public override void ResetLayout()
        {
            // Prevents errors when not all children have been added yet.
            if (_children.Count > 1)
            {
                var absoluteRect = AbsoluteRectangle;

                if (SplitDirection == SplitDirection.Vertical)
                {
                    if (FixedChild == 0)
                    {
                        // A vertically fixed first child starts at (0,0), with a width equal to the split size.
                        _children[0].RelativeRectangle = new Rectangle(
                            Point.Zero,
                            new Point(_split, absoluteRect.Height));

                        // A vertically non-fixed second child starts at the end of the first child and takes up all remaining width of the parent.
                        _children[1].RelativeRectangle = new Rectangle(
                            new Point(_split, 0),
                            new Point(absoluteRect.Width - _split, absoluteRect.Height));
                    }
                    else
                    {
                        // A vertically non-fixed first child starts at (0,0), with a width equal to the total parent width minus the split size.
                        _children[0].RelativeRectangle = new Rectangle(
                            Point.Zero,
                            new Point(absoluteRect.Width - _split, absoluteRect.Height));

                        // A vertically fixed second child starts at the end of the first child and takes up the split size in width.
                        _children[1].RelativeRectangle = new Rectangle(
                            new Point(absoluteRect.Width - _split, 0),
                            new Point(_split, absoluteRect.Height));
                    }
                }
                else if (SplitDirection == SplitDirection.Horizontal)
                {
                    if (FixedChild == 1)
                    {
                        // A horizontally fixed first child starts at (0,0), with a height equal to the split size.
                        _children[0].RelativeRectangle = new Rectangle(
                            Point.Zero,
                            new Point(absoluteRect.Width, _split));

                        // A horizontally non-fixed second child starts at the end of the first child and takes up all remaining height.
                        _children[1].RelativeRectangle = new Rectangle(
                            new Point(0, _split),
                            new Point(absoluteRect.Width, absoluteRect.Height - _split));
                    }
                    else
                    {
                        // A horizontally non-fixed first child starts at (0,0), with a height equal to the total parent height minus the split size.
                        _children[0].RelativeRectangle = new Rectangle(
                            Point.Zero,
                            new Point(absoluteRect.Width, absoluteRect.Height - _split));

                        // A horizontally fixed second child starts at the end of the first child and takes up the split size in height.
                        _children[1].RelativeRectangle = new Rectangle(
                            new Point(0, absoluteRect.Height - _split),
                            new Point(absoluteRect.Width, _split));
                    }
                }
            }
            base.ResetLayout();
        }

        /// <summary>
        ///     Add a child element to the split panel.
        /// </summary>
        public override void AddChild(UIElement child)
        {
            if (_children.Count == 2)
            {
                throw new OrbisUIException("Can not add more than two children to a split panel.");
            }

            _children.Add(child);

            base.AddChild(child);
        }

        /// <summary>
        ///     Replace one of the split panel's children with a new one.
        /// </summary>
        /// <param name="childIndex">
        ///     The index of the child to replace.
        /// </param>
        /// <param name="child">
        ///     The element to replace it with.
        /// </param>
        public override void ReplaceChild(int childIndex, UIElement newChild)
        {
            // Split panels can only have two children.
            if (childIndex != 0 && childIndex != 1)
            {
                throw new OrbisUIException("Invalid child index for split panel.");
            }

            _children[childIndex] = newChild;

            base.ReplaceChild(childIndex, newChild);
        }
    }
}
