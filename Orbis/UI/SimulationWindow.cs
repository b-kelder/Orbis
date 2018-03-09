using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI
{
    public class SimulationWindow : UIElement
    {
        /// <summary>
        ///     The children of this UI Element.
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
        ///     The viewPort where the Simulation can be drawn.
        /// </summary>
        public Viewport SimulationViewport
        {
            get
            {
                return _simView;
            }
        }
        private Viewport _simView;

        /// <summary>
        ///     The default viewport.
        /// </summary>
        public Viewport DefaultViewport {
            get
            {
                return _defaultViewport;
            }
        }
        private Viewport _defaultViewport;

        /// <summary>
        ///     Create a new <see cref="SimulationWindow"/>.
        /// </summary>
        public SimulationWindow(Viewport defaultViewport)
        {
            _defaultViewport = defaultViewport;
            _children = new List<UIElement>();
            _simView = defaultViewport;
        }

        /// <summary>
        ///     Used when the layout of the UI needs to update.
        /// </summary>
        public override void UpdateLayout()
        {
            _simView.Bounds = AbsoluteRectangle;
            base.UpdateLayout();
        }
    }
}
