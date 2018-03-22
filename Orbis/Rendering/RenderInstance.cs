using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// An instance of a RenderableMesh to render with specific settings.
    /// </summary>
    struct RenderInstance
    {
        public RenderableMesh mesh;
        public Matrix matrix;
    }
}
