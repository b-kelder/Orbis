using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbis.UI.Utility
{
    /// <summary>
    ///     Loads and manages textures for the UI.
    /// </summary>
    public class UIContentManager
    {
        // Used to load new textures.
        private ContentManager _contentManager;

        // Keeps track of existing color textures.
        private Dictionary<Color, Texture2D> _loadedColorTextures;

        // Used to store already loaded textures.
        private Dictionary<string, SpriteFont> _loadedFonts;

        // Used to store already loaded textures.
        private Dictionary<string, Texture2D> _loadedTextures;

        // Used for creating textures.
        private GraphicsDevice _graphicsDevice;

        // Global instance.
        private static UIContentManager _instance;

        /// <summary>
        ///     Create a new <see cref="UIContentManager"/>.
        /// </summary>
        /// 
        /// <param name="game">
        ///     The current Game object.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException" />
        private UIContentManager(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException();
            }

            _graphicsDevice = game.GraphicsDevice;

            _contentManager = new ContentManager(game.Services, "Content");

            _loadedColorTextures = new Dictionary<Color, Texture2D>();

            _loadedFonts = new Dictionary<string, SpriteFont>();

            _loadedTextures = new Dictionary<string, Texture2D>();
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
        /// <param name="game">
        ///     The current Game object.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException" />
        public static void CreateInstance(Game game)
        {
            _instance = (_instance == null) ? new UIContentManager(game)
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
        public Texture2D GetColorTexture(Color color)
        {

            if (!_loadedColorTextures.TryGetValue(color, out Texture2D texture))
            {
                texture = new Texture2D(_graphicsDevice, 1, 1);
                texture.SetData(new Color[] { color });
                _loadedColorTextures.Add(color, texture);
            }

            return texture;
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
        public void UnloadAll()
        {
            // The textures are unloaded.
            foreach (KeyValuePair<string, Texture2D> entry in _loadedTextures)
            {
                entry.Value.Dispose();
            }
            _loadedTextures.Clear();

            foreach (KeyValuePair<Color, Texture2D> entry in _loadedColorTextures)
            {
                entry.Value.Dispose();
            }
            _loadedColorTextures.Clear();

            _loadedFonts.Clear();

            // Also unload the content manager.
            _contentManager.Unload();
        }
    }
}
