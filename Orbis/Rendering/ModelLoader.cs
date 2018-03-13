using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    class ModelLoader
    {
        /// <summary>
        /// Loads a model from the given mesh and texture files and makes it renderable.
        /// </summary>
        /// <param name="meshFile">File of the mesh</param>
        /// <param name="textureFile">File of the texture</param>
        /// <param name="shader">Effect to use for material</param>
        /// <param name="device">GraphicsDevice used to render</param>
        /// <returns></returns>
        public static Model LoadModel(string meshFile, string textureFile, string colorTextureFile, Effect shader, GraphicsDevice device)
        {
            Mesh mesh;
            Material material = new Material(shader);
            using(var stream = TitleContainer.OpenStream(meshFile))
            {
                mesh = ObjParser.FromStream(stream);
            }
            if(textureFile != null)
            {
                using (var stream = TitleContainer.OpenStream(textureFile))
                {
                    material.Texture = Texture2D.FromStream(device, stream);
                }
            }
            if(colorTextureFile != null)
            {
                using (var stream = TitleContainer.OpenStream(colorTextureFile))
                {
                    material.ColorMap = Texture2D.FromStream(device, stream);
                }
            }
            return new Model(mesh, material);
        }
    }
}
