using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    class Material
    {
        public Texture2D Texture { get; set; }
        public BasicEffect Effect { get; set; }

        public Material(BasicEffect effect)
        {
            Effect = effect;
        }
    }
}
