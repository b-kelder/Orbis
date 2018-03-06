using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI
{
    /// <summary>
    ///     Represents a drawable panel in the UI.
    /// </summary>
    public class UIPanel : UIElement
    {
        /// <summary>
        ///     The sprite batch used by this element to draw itself.
        /// </summary>
        public SpriteBatch SpriteBatch { get; set; }

        /// <summary>
        ///     The background texture for this panel.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        ///     Create a new <see cref="UIPanel"/> without a parent and texture;
        /// </summary>
        public UIPanel() : base() { }

        /// <summary>
        ///     Create a new <see cref="UIPanel"/> without parent and texture;
        /// </summary>
        public UIPanel(int x, int y, int width, int height) : base(x, y, width, height) { }

        /// <summary>
        ///     Create a new <see cref="UIPanel"/> with the given texture
        /// </summary>
        public UIPanel(int x, int y, int width, int height, SpriteBatch spriteBatch, Texture2D texture) : base(x, y, width, height)
        {
            SpriteBatch = spriteBatch;
            BackgroundTexture = texture;
        }

        /// <summary>
        ///     Draw the panel.
        /// </summary>
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Draw(GameTime gameTime)
        {
            if (SpriteBatch != null && BackgroundTexture != null)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(BackgroundTexture, _elementRect, Color.White);
                SpriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
