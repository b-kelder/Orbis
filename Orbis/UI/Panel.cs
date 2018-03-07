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
    public class Panel : UIElement
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
        ///     The children of this UI Element.
        /// </summary>
        public override UIElement[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }
        private List<UIElement> _children;

        /// <summary>
        ///     Create a new <see cref="Panel"/>.
        /// </summary>
        public Panel() : base() {
            _children = new List<UIElement>();
        }

        /// <summary>
        ///     Draw the panel.
        /// </summary>
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Draw(GameTime gameTime)
        {
            if (SpriteBatch != null && BackgroundTexture != null && Visible)
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(BackgroundTexture, this.AbsoluteRectangle, Color.White);
                SpriteBatch.End();
            }
            base.Draw(gameTime);
        }

        /// <summary>
        ///     Add a child to the panel.
        /// </summary>
        public override void AddChild(UIElement child)
        {
            _children.Add(child);

            CheckElementBoundaries(child.AbsoluteRectangle, this.AbsoluteRectangle);

            base.AddChild(child);
        }
    }
}
