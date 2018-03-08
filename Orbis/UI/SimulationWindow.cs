using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI
{
    public class SimulationWindow : Panel
    {
        public override Texture2D BackgroundTexture
        {
            get
            {
                return _simRenderTarget;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     The render target that the simulation will be rendered to.
        /// </summary>
        public RenderTarget2D SimulationRenderTarget
        {
            get
            {
                return _simRenderTarget;
            }
            set
            {
                _simRenderTarget = value;
            }
        }
        private RenderTarget2D _simRenderTarget;

        /// <summary>
        ///     The graphicsDevice used to render the simulation.
        /// </summary>
        private GraphicsDevice _graphicsDevice;

        public SimulationWindow(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _simRenderTarget = new RenderTarget2D(_graphicsDevice, 1, 1);
        }

        public override void ResetLayout()
        {
            if (Size.X > 0 && Size.Y > 0)
            {
                _simRenderTarget.Dispose();
                _simRenderTarget = new RenderTarget2D(_graphicsDevice, Size.X, Size.Y);
            }
            base.ResetLayout();
        }
    }
}
