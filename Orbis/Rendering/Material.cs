using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Stores related texture and shader data
    /// </summary>
    class Material
    {
        /// <summary>
        /// The main diffuse texture
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// The vertex color blend texture
        /// </summary>
        public Texture2D ColorMap { get; set; }
        /// <summary>
        /// The effect/shader to use
        /// </summary>
        public Effect Effect { get; set; }

        /// <summary>
        /// Creates a new material with the given shader
        /// </summary>
        /// <param name="effect">Shader</param>
        public Material(Effect effect)
        {
            Effect = effect;
        }
    }
}
