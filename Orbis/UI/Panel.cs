using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI
{
    /// <summary>
    ///     Represents a drawable panel in the Orbis UI.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class Panel : UIElement
    {
        /// <summary>
        ///     The background texture for this panel.
        /// </summary>
        public virtual Texture2D BackgroundTexture { get; set; }

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
        /// 
        /// <param name="spriteBatch">
        ///     The SpriteBatch to use for drawing textures.
        /// </param>
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Visible)
            {
                // Drawing is only done if the required resources are set.
                if (spriteBatch != null && BackgroundTexture != null)
                {
                    spriteBatch.Draw(BackgroundTexture,
                        AbsoluteRectangle,
                        null,
                        Color.White,
                        0.00F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth);
                }
                base.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        ///     Add a child to the panel.
        /// </summary>
        public override void AddChild(UIElement child)
        {
            _children.Add(child);
            
            base.AddChild(child);
        }

        /// <summary>
        ///     Replace one of the panel's children.
        /// </summary>
        /// 
        /// <param name="childIndex">
        ///     The index of the child to replace.
        /// </param>
        /// <param name="newChild">
        ///     The child to replace it with.
        /// </param>
        /// 
        /// <exception cref="OrbisUIException" />
        public override void ReplaceChild(int childIndex, UIElement newChild)
        {
            if (childIndex < 0 || childIndex >= _children.Count)
            {
                throw new OrbisUIException("Child index out of range.");
            }

            Children[childIndex] = newChild;

            base.ReplaceChild(childIndex, newChild);
        }
    }
}
