using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orbis.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orbis.UI
{
    /// <summary>
    ///     A button in the Orbis UI.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class Button : UIElement
    {
        /// <summary>
        ///     Get the button's children (it has none).
        /// </summary>
        public override UIElement[] Children
        {
            get
            {
                return new UIElement[0];
            }
        }
        
        /// <summary>
        ///     The background texture to use for drawing the button.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        ///     The text to display on the button.
        /// </summary>
        public string Text {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                UpdateLayout();
            }
        }
        private string _text;
        private string _visibleText;

        /// <summary>
        ///     The font used to draw the button text.
        /// </summary>
        public SpriteFont TextFont { get; set; }

        /// <summary>
        ///     The color of the button text.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        ///     The action to perform when the button is clicked.
        /// </summary>
        public Action OnClick { get; set; }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Buttons don't respond when invisible or when no click action has been set.
            if (Visible && OnClick != null)
            {
                MouseState mouseState = Mouse.GetState();
                if (AbsoluteRectangle.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                {
                    OnClick();
                }
            }
            // No base draw required; buttons have no children.
        }

        /// <summary>
        ///     Create a new <see cref="Button"/>.
        /// </summary>
        public Button()
        {
            Text = "";
            TextColor = Color.Black;
        }

        /// <summary>
        ///     Draw the button on the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Drawing is done only if the required resources are set.
            if (spriteBatch != null && BackgroundTexture != null)
            {
                spriteBatch.Draw(BackgroundTexture, AbsoluteRectangle, Color.White);

                // No text is drawn if no font is set or if the text is empty since there is no point to doing it in those cases.
                if (TextFont != null && !string.IsNullOrWhiteSpace(Text))
                {
                    // Before drawing the string, it is wrapped to fit within the button.
                    Vector2 textSize = TextFont.MeasureString(_visibleText);

                    // Ensure that the absolute rectangle is only calculated once for this calculation.
                    Point absoluteCenter = AbsoluteRectangle.Center;
                    spriteBatch.DrawString(TextFont, _visibleText, new Vector2(absoluteCenter.X - textSize.X / 2, absoluteCenter.Y - textSize.Y / 2), TextColor);
                }
            }
        }

        /// <summary>
        ///     Updates the layout of the <see cref="Button"/>.
        /// </summary>
        public override void UpdateLayout()
        {
            if (TextFont != null && !string.IsNullOrWhiteSpace(Text) && Size.X > 0 && Size.Y > 0)
            {
                // The text is wrapped to fit in the button.
                Vector2 textSize;
                _visibleText = Text;
                bool fits;
                do
                {
                    textSize = TextFont.MeasureString(_visibleText);
                    if (textSize.X > Size.X - 4)
                    {
                        _visibleText.Substring(0, _visibleText.Length - 1);
                        fits = false;
                    }
                    else
                    {
                        fits = true;
                    }
                } while (!fits);
            }
            // No base UpdateLayout needed; buttons have no children.
        }
    }
}
