using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    struct RenderInstance
    {
        public RenderableMesh mesh;
        public Material material;
        public Matrix matrix;
    }
}
