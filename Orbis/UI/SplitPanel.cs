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
        ///     Should be between 0 and 1.
        /// </summary>
        public float Split
        {
            get
            {
                return _split;
            }
            set
            {
                var clamped = MathHelper.Clamp(value, 0.01F, 0.99F);
                if (_split != clamped)
                {
                    _split = clamped;
                    ResetLayout();
                }
            }
        }
        private float _split;

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
            _split = .5F;
            _splitDirection = SplitDirection.Vertical;
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
                    _children[0].RelativeRectangle = new Rectangle(
                        Point.Zero,
                        new Point((int)Math.Floor(absoluteRect.Width * _split), absoluteRect.Height));
                    if (_children.Count > 1)
                    {
                        _children[1].RelativeRectangle = new Rectangle(
                            new Point((int)Math.Floor(absoluteRect.Width * _split), 0),
                            new Point((int)Math.Floor(absoluteRect.Width * _split), absoluteRect.Height));
                    }
                }
                else if (SplitDirection == SplitDirection.Horizontal)
                {
                    _children[0].RelativeRectangle = new Rectangle(
                        Point.Zero,
                        new Point(absoluteRect.Width, (int)Math.Floor(absoluteRect.Height * _split)));
                    if (_children.Count > 1)
                    {
                        _children[1].RelativeRectangle = new Rectangle(
                            new Point(0, (int)Math.Floor(absoluteRect.Height * _split)),
                            new Point(absoluteRect.Width, (int)Math.Floor(absoluteRect.Height * _split)));
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
