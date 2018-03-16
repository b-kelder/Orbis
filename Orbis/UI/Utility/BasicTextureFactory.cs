using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Orbis.UI.Utility
{
    /// <summary>
    ///     A factory for creating and managing single-color basic textures.
    /// </summary>
    /// 
    /// <remarks>
    ///     This is essentially a debug class and shouldn't be included in the final project,
    ///     as it is just a utility for easy placeholder texture creation.
    /// </remarks>
    public class BasicTextureFactory
    {
        // Keeps track of existing colors.
        private Dictionary<Color, Texture2D> _loadedColors;

        // Global instance.
        private static BasicTextureFactory _instance;

        // Used for creating textures.
        private GraphicsDevice _graphicsDevice;

        /// <summary>
        ///     Create the <see cref="BasicTextureFactory"/>.
        /// </summary>
        private BasicTextureFactory(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _loadedColors = new Dictionary<Color, Texture2D>();
        }

        /// <summary>
        ///     Attempt to get the instance for the <see cref="BasicTextureFactory"/>.
        /// </summary>
        /// 
        /// <param name="instance">
        ///     The instance that will be retrieved.
        /// </param>
        /// 
        /// <returns>
        ///     A boolean indicatint the success of retrieval.
        /// </returns>
        public static bool TryGetInstance(out BasicTextureFactory instance)
        {
            instance = _instance;

            return (instance != null) ? true : false;
        }

        /// <summary>
        ///     Create the global instance of the <see cref="BasicTextureFactory"/>.
        /// </summary>
        /// 
        /// <param name="graphicsDevice">
        ///     The graphicsDevice that will be used for creating textures.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException" />
        public static void CreateInstance(GraphicsDevice graphicsDevice)
        {
            _instance = (_instance == null) ? new BasicTextureFactory(graphicsDevice)
                : throw new InvalidOperationException("Instance already exists.");
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

            if (!_loadedColors.TryGetValue(color, out Texture2D texture))
            {
                texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new Color[] { color });
                _loadedColors.Add(color, texture);
            }

            return texture;
        }

        /// <summary>
        ///     Clean up all resources used by the <see cref="BasicTextureFactory"/>.
        /// </summary>
        public void UnloadAll()
        {
            foreach (KeyValuePair<Color, Texture2D> entry in _loadedColors)
            {
                entry.Value.Dispose();
            }
            _loadedColors.Clear();
        }
    }
}
