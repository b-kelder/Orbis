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

        public RenderTarget2D SimulationRenderTarget
        {
            get
            {
                return _simRenderTarget;
            }
        }
        private RenderTarget2D _simRenderTarget;
    }
}
