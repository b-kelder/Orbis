using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.UI
{
    /// <summary>
    ///     Represents a scrollbar that can be used by UI Elements.
    /// </summary>
    public class Scrollbar : UIElement
    {
        /// <summary>
        ///     Get the scrollbar's children (it has none).
        /// </summary>
        public override UIElement[] Children
        {
            get
            {
                return new UIElement[0];
            }
        }

        /// <summary>
        ///     The texture used as the background for the scrollbar.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        ///     The texture used for the handle of the scrollbar.
        /// </summary>
        public Texture2D HandleTexture { get; set; }

        /// <summary>
        ///     The texture used for the buttons of the scrollbar.
        /// </summary>
        /// 
        /// <remarks>
        ///     The proper texture for a scroll button is a 15x15 upward-facing arrow texture.
        /// </remarks>
        public Texture2D ButtonTexture { get; set; }

        /// <summary>
        ///     The position of the scrollbar's handle.
        /// </summary>
        public float HandlePosition { get; private set; }

        /// <summary>
        ///     Is the scrollbar horizontal?
        /// </summary>
        public bool IsHorizontal { get; set; }

        /// <summary>
        ///     Create a new <see cref="Scrollbar"/>.
        /// </summary>
        public Scrollbar()
        {
            HandlePosition = 50.00F;
        }

        /// <summary>
        ///     Perform update actions for the scrollbar.
        /// </summary>
        /// 
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw only if the scrollbar is visible.
            if (Visible)
            {
                // Drawing will not happen if the required resources are not assigned.
                if (spriteBatch != null && BackgroundTexture != null && HandleTexture != null && ButtonTexture != null)
                {
                    
                }
            }
        }

        /// <summary>
        ///     Scrollbars cannot have children; do not use.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public override void AddChild(UIElement child)
        {
            throw new OrbisUIException("Scrollbars can not have children.");
        }

        /// <summary>
        ///     Scrollbars can not have children; do not use.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public override void ReplaceChild(int childIndex, UIElement newChild)
        {
            throw new OrbisUIException("Scrollbars can not have children.");
        }

        /// <summary>
        ///     Update the layout of the scroll bar.
        /// </summary>
        public override void UpdateLayout()
        {
            base.UpdateLayout();
        }
    }
}
