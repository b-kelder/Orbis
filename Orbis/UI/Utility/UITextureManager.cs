using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbis.UI
{
    /// <summary>
    ///     Loads and manages textures for the UI.
    /// </summary>
    public class UIContentManager : IDisposable
    {
        // Used to load new textures.
        private ContentManager _contentManager;

        // Used to store already loaded textures.
        private Dictionary<string, SpriteFont> _loadedFonts;

        // Used to store already loaded textures.
        private Dictionary<string, Texture2D> _loadedTextures;

        // Global instance.
        private static UIContentManager _instance;

        /// <summary>
        ///     Create a new <see cref="UIContentManager"/>.
        /// </summary>
        /// 
        /// <param name="provider">
        ///     The serviceprovider to use for creating the content manager.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException" />
        private UIContentManager(IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException();
            }

            _loadedTextures = new Dictionary<string, Texture2D>();
            _contentManager = new ContentManager(provider, "Content");
        }

        /// <summary>
        ///     Attempt to get the instance for the manager.
        /// </summary>
        /// 
        /// <param name="instance">
        ///     The instance that will be retrieved.
        /// </param>
        /// 
        /// <returns>
        ///     A boolean indicatint the success of retrieval.
        /// </returns>
        public static bool TryGetInstance(out UIContentManager instance)
        {
            instance = _instance;

            return (instance != null) ? true : false;
        }

        /// <summary>
        ///     Create the global instance of the UITextureManager.
        /// </summary>
        /// 
        /// <param name="serviceProvider">
        ///     The serviceprovider that will be used for loading content.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException" />
        public static void CreateInstance(IServiceProvider serviceProvider)
        {
            _instance = (_instance == null) ? new UIContentManager(serviceProvider)
                : throw new InvalidOperationException("Instance already exists.");
        }

        /// <summary>
        ///     Get the UI texture with the given name.
        /// </summary>
        /// 
        /// <param name="name">
        ///     The name of the texture to get.
        /// </param>
        /// 
        /// <returns>
        ///     The loaded texture.
        /// </returns>
        public Texture2D GetTexture(string name)
        {
            // Get already loaded texture if available, load new one if not available.
            if (!_loadedTextures.TryGetValue(name, out Texture2D texture))
            {
                texture = _contentManager.Load<Texture2D>(name);
                _loadedTextures.Add(name, texture);
            }

            return texture;
        }

        /// <summary>
        ///     Get a font with the given name.
        /// </summary>
        /// 
        /// <param name="name">
        ///     The name of the font to retrieve.
        /// </param>
        /// 
        /// <returns>
        ///     The loaded font.
        /// </returns>
        public SpriteFont GetFont(string name)
        {
            if (!_loadedFonts.TryGetValue(name, out SpriteFont font))
            {
                font = _contentManager.Load<SpriteFont>(name);
                _loadedFonts.Add(name, font);
            }

            return font;
        }

        /// <summary>
        ///     Clean up all resources used by the <see cref="UIContentManager"/>.
        /// </summary>
        public void Dispose()
        {
            // The textures are what takes up resources in this class.
            foreach (KeyValuePair<string, Texture2D> entry in _loadedTextures)
            {
                entry.Value.Dispose();
            }
            _loadedTextures.Clear();
        }
    }
}
