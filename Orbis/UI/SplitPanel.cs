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
                if (value != 1 && value != 2)
                {
                    throw new OrbisUIException("Fixed child index out of range.");
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

        /// <summary>
        ///     Reset the layout of the <see cref="SplitPanel"/> and all its children.
        /// </summary>
        public override void ResetLayout()
        {
            if (_children.Count > 0)
            {
                var absoluteRect = AbsoluteRectangle;

                if (SplitDirection == SplitDirection.Vertical)
                {
                    if (FixedChild == 1)
                    {
                        _children[0].RelativeRectangle = new Rectangle(
                        Point.Zero,
                        new Point(_split, absoluteRect.Height));
                        if (_children.Count > 1)
                        {
                            _children[1].RelativeRectangle = new Rectangle(
                                new Point(_split, 0),
                                new Point(absoluteRect.Width - _split, absoluteRect.Height));
                        }
                    }
                    else
                    {
                        _children[0].RelativeRectangle = new Rectangle(
                        Point.Zero,
                        new Point(absoluteRect.Width - _split, absoluteRect.Height));
                        if (_children.Count > 1)
                        {
                            _children[1].RelativeRectangle = new Rectangle(
                                new Point(absoluteRect.Width - _split, 0),
                                new Point(_split, absoluteRect.Height));
                        }
                    }
                    
                }
                else if (SplitDirection == SplitDirection.Horizontal)
                {
                    if (FixedChild == 1)
                    {
                        _children[0].RelativeRectangle = new Rectangle(
                        Point.Zero,
                        new Point(absoluteRect.Width, _split));
                        if (_children.Count > 1)
                        {
                            _children[1].RelativeRectangle = new Rectangle(
                                new Point(0, _split),
                                new Point(absoluteRect.Width, absoluteRect.Height - _split));
                        }
                    }
                    else
                    {
                        _children[0].RelativeRectangle = new Rectangle(
                        Point.Zero,
                        new Point(absoluteRect.Width, absoluteRect.Height - _split));
                        if (_children.Count > 1)
                        {
                            _children[1].RelativeRectangle = new Rectangle(
                                new Point(0, absoluteRect.Height - _split),
                                new Point(absoluteRect.Width, _split));
                        }
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
    }
}
