using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.Engine;
using System;
using System.Linq;

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

        // The absolute position and size of the buttons used to navigate the scrollbar.
        private Rectangle _upButton;
        private Rectangle _handle;
        private Rectangle _downButton;

        /// <summary>
        ///     Create a new <see cref="Scrollbar"/>.
        /// </summary>
        public Scrollbar()
        {
            HandlePosition = 0F;
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
            // Scrollbar handles input through the buttons at the top and bottom.
            InputHandler input = InputHandler.GetInstance();
            Point mousePosition = Mouse.GetState().Position;
            if (_upButton.Contains(mousePosition))
            {
                if (input.IsMouseHold(MouseButton.Left))
                {
                    HandlePosition = MathHelper.Clamp(HandlePosition - 1F, 0, 100);
                    UpdateLayout();
                }
            }
            else if (_downButton.Contains(mousePosition))
            {
                if (input.IsMouseHold(MouseButton.Left))
                {
                    HandlePosition = MathHelper.Clamp(HandlePosition + 1F, 0, 100);
                    UpdateLayout();
                }
            }
            // No base Update needed; scrollbars do not have children.
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw only if the scrollbar is visible.
            if (Visible)
            {
                // Drawing will not happen if the required resources are not assigned.
                if (spriteBatch != null && BackgroundTexture != null && HandleTexture != null && ButtonTexture != null)
                {
                    spriteBatch.Draw(BackgroundTexture,
                        AbsoluteRectangle,
                        null,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth);

                    spriteBatch.Draw(ButtonTexture,
                        _upButton,
                        null,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth - 0.001F);

                    spriteBatch.Draw(HandleTexture,
                        _handle,
                        null,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth - 0.001F);

                    spriteBatch.Draw(ButtonTexture,
                        _downButton,
                        null,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        SpriteEffects.FlipVertically,
                        LayerDepth - 0.001F);
                }
            }
        }

        /// <summary>
        ///     Scrollbars can not have children; do not use.
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
            // Don't bother updating these things if the scrollbar isn't visible.
            if (Visible)
            {
                // Ensure the absolute position is only calculated once.
                Rectangle absoluteRectangle = AbsoluteRectangle;

                // Set the positions of the buttons and handle based on the absolute position of the scrollbar.
                _upButton = new Rectangle(absoluteRectangle.X, absoluteRectangle.Y, 15, 15);
                int handleY = (int)Math.Floor(absoluteRectangle.Y + ((Size.Y - 50) * (HandlePosition / 100)));
                _handle = new Rectangle(absoluteRectangle.X, handleY + 15, 15, 20);
                _downButton = new Rectangle(absoluteRectangle.X, absoluteRectangle.Bottom - 15, 15, 15);
            }

            // No base UpdateLayout needed; scrollbars do not have children;
        }
    }
}
