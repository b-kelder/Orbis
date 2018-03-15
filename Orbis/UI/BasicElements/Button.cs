using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.Engine;
using System;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     A button in the Orbis UI.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class Button : PositionedTexture
    {
        /// <summary>
        ///     The layer depth of the button.
        /// </summary>
        /// 
        /// <remarks>
        ///     Value should be between 0 and 1, with zero being the front and 1 being the back.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException" />
        public override float LayerDepth
        {
            get
            {
                return base.LayerDepth;
            }
            set
            {
                base.LayerDepth = value;

                if (_text != null)
                {
                    // Place the text above the background.
                    _text.LayerDepth = LayerDepth - 0.001F;
                }
            }
        }

        /// <summary>
        ///     The size of the button.
        /// </summary>
        public override Point Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        /// <summary>
        ///     The text displayed on the button.
        /// </summary>
        public string Text
        {
            get
            {
                return (_text != null) ? _textString : "";
            }
            set
            {
                if (_text != null)
                {
                    _textString = value;
                }
            }
        }
        private string _textString;
        private PositionedText _text;

        /// <summary>
        ///     The color of the text to draw in the button.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (_text != null) ? _text.TextColor : Color.Transparent;
            }
            set
            {
                if (_text != null)
                {
                    _text.TextColor = value;
                }
            }
        }

        /// <summary>
        ///     The onscreen position to check for mouse input.
        /// </summary>
        public Rectangle SceenPosition { get; set; }

        /// <summary>
        ///     Fires when the button has been clicked.
        /// </summary>
        public event EventHandler ButtonClickedEvent;

        /// <summary>
        ///     Perform the button's update for this frame.
        /// </summary>
        public void Update()
        {
            InputHandler input = InputHandler.GetInstance();
            Point mousePosition = Mouse.GetState().Position;
            if (SceenPosition.Contains(mousePosition) && input.IsMouseReleased(MouseButton.Left))
            {
                ButtonClickedEvent.Invoke(this, null);
            }
        }

        /// <summary>
        ///     Create a new <see cref="Button"/>.
        /// </summary>
        public Button(Texture2D texture) : base(texture)
        {
            // No action required.
        }

        /// <summary>
        ///     Create a new <see cref="Button"/> with text.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        public Button(Texture2D texture, SpriteFont font, string text) : base(texture)
        {
            // Create text slightly in front of the background.
            _text = new PositionedText(font);
            Text = text;
        }

        /// <summary>
        ///     Render the button with the given spriteBatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Render(SpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);

            if (_text != null)
            {
                int maxWidth = (int)Size.X - 8;
                SpriteFont font = _text.Font;
                string clippedString = Utility.TextHelper.ClipText(font, Text, maxWidth);
                Vector2 textSize = font.MeasureString(clippedString);

                Point center = Bounds.Center;
                _text.Position = new Point((int)Math.Floor(center.X - textSize.X / 2), (int)Math.Floor(center.Y - textSize.Y / 2));
                _text.Text = clippedString;

                _text.Render(spriteBatch);
            }
        }

        ///// <summary>
        /////     Draw the button on the screen.
        ///// </summary>
        ///// <param name="spriteBatch"></param>
        ///// <param name="gameTime"></param>
        //public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    // Drawing is done only if the required resources are set.
        //    if (spriteBatch != null && BackgroundTexture != null)
        //    {
        //        spriteBatch.Draw(BackgroundTexture,
        //            AbsoluteRectangle,
        //            null,
        //            Color.White,
        //            0.00F,
        //            Vector2.Zero,
        //            SpriteEffects.None,
        //            LayerDepth);

        //        // No text is drawn if no font is set or if the text is empty since there is no point to doing it in those cases.
        //        if (TextFont != null && !string.IsNullOrWhiteSpace(Text))
        //        {
        //            // Before drawing the string, it is wrapped to fit within the button.
        //            Vector2 textSize = TextFont.MeasureString(_visibleText);

        //            // Ensure that the absolute rectangle is only calculated once for this calculation.
        //            Point absoluteCenter = AbsoluteRectangle.Center;
        //            Vector2 textPos = new Vector2(absoluteCenter.X - textSize.X / 2, absoluteCenter.Y - textSize.Y / 2);
        //            spriteBatch.DrawString(TextFont,
        //                _visibleText,
        //                textPos,
        //                TextColor,
        //                0.00F,
        //                Vector2.Zero,
        //                1.00F,
        //                SpriteEffects.None,
        //                LayerDepth - 0.001F);
        //        }
        //    }
        //}

        ///// <summary>
        /////     Updates the layout of the <see cref="Button"/>.
        ///// </summary>
        //public override void UpdateLayout()
        //{
        //    if (TextFont != null && !string.IsNullOrWhiteSpace(Text) && Size.X > 0 && Size.Y > 0)
        //    {
               


        //    }
        //    // No base UpdateLayout needed; buttons have no children.
        //}
    }
}
