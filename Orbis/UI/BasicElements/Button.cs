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
    public class Button : PositionedTexture, IUpdatableElement
    {
        /// <summary>
        ///     Fires when the button has been clicked.
        /// </summary>
        public event EventHandler ButtonClickedEvent;

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
        ///     The onscreen position to check for mouse input.
        /// </summary>
        public Rectangle SceenPosition { get; set; }

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
    }
}
