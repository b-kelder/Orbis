using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Combination of Mesh and Material.
    /// </summary>
    class Model
    {
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }

        public Model(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }
    }
}
