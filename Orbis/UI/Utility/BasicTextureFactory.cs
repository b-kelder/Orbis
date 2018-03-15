using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI.Utility
{
    /// <summary>
    ///     A factory for creating and managing single-color basic textures.
    /// </summary>
    public class BasicTextureFactory : IDisposable
    {
        // Used for creating textures.
        private GraphicsDevice _graphicsDevice;

        // Keeps track of existing colors.
        private Dictionary<Color, Texture2D> _colors;

        /// <summary>
        ///     Static constructor for the <see cref="BasicTextureFactory"/>.
        /// </summary>
        public BasicTextureFactory(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _colors = new Dictionary<Color, Texture2D>();
        }

        /// <summary>
        ///     Create a single color texture with the given color.
        /// </summary>
        /// 
        /// <param name="graphicsDevice">
        ///     The graphics device to use for creating the texture.
        /// </param>
        /// <param name="color">
        ///     The color to use for the texture.
        /// </param>
        public Texture2D CreateBasicTexture(Color color)
        {

            if (!_colors.TryGetValue(color, out Texture2D texture))
            {
                texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new Color[] { color });
                _colors.Add(color, texture);
            }

            return texture;
        }

        /// <summary>
        ///     Clean up all resources used by the <see cref="BasicTextureFactory"/>.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<Color, Texture2D> entry in _colors)
            {
                entry.Value.Dispose();
            }
            _colors.Clear();
        }
    }
}
