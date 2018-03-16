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
    public class Button : RelativeElement, IUpdatableElement
    {
        // Does the button have text?
        private Boolean _hasText;

        // Used to draw the background of the button.
        private RelativeTexture _texture;

        // Used to draw the text on the button.
        private RelativeText _text;

        /// <summary>
        ///     Fires when the area was clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        ///     Fires when the mouse is held down over the area.
        /// </summary>
        public event EventHandler Hold;

        /// <summary>
        ///     The font to use for the button text.
        /// </summary>
        public SpriteFont Font
        {
            get
            {
                return (_hasText) ? _text.Font : null;
            }
            set
            {
                if (_hasText)
                {
                    _text.Font = value;
                }
            }
        }


        /// <summary>
        ///     Is the button focused?.
        /// </summary>
        public bool IsFocused { get; set; }

        /// <summary>
        ///     The layer depth of the button.
        /// </summary>
        /// 
        /// <remarks>
        ///     Value should be between 0 and 1, with zero being the front and 1 being the back.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException" />
        public float LayerDepth {
            get
            {
                return _texture.LayerDepth;
            }
            set
            {
                _texture.LayerDepth = value;

                if (_hasText)
                {
                    _text.LayerDepth = value - 0.001F;
                }
            }
        }

        ///// <summary>
        /////     The relative position of the button.
        ///// </summary>
        //public Point Position { get => _texture.Position; set => _texture = value; }

        /// <summary>
        ///     The dimensions of the button.
        /// </summary>
        public Point Size { get => _texture.Size; set => _texture.Size = value; }

        /// <summary>
        ///     The effects to use when drawing the button.
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get
            {
                return _texture.SpriteEffects;
            }
            set
            {
                _texture.SpriteEffects = value;

                if (_hasText)
                {
                    _text.SpriteEffects = value;
                }
            }
        }

        /// <summary>
        ///     The on-screen area filled by the button.
        /// </summary>
        public Rectangle ScreenArea { get; set; }

        /// <summary>
        ///     The color of the button text.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (_hasText) ? _text.TextColor : Color.White;
            }
            set
            {
                if (_hasText)
                {
                    _text.TextColor = value;
                }
            }
        }

        /// <summary>
        ///     The sprite to use for drawing the button.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public SpriteDefinition SpriteDefinition { get => _texture.SpriteDefinition; set => _texture.SpriteDefinition = value; }

        /// <summary>
        ///     The text displayed on the button.
        /// </summary>
        public string Text { get => _textString; set => _textString = value; }
        private string _textString;

        /// <summary>
        ///     Create a new <see cref="Button"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public Button(IPositionedElement parent, SpriteDefinition spriteDefinition, SpriteFont font = null) : base(parent)
        {
            _texture = new RelativeTexture(this, spriteDefinition);
            if (font != null)
            {
                _text = new RelativeText(this, font)
                {
                    AnchorPosition = AnchorPosition.Center
                };
                _hasText = true;
            }
            else
            {
                _hasText = false;
            }
            IsFocused = false;
            ScreenArea = Rectangle.Empty;
        }

        /// <summary>
        ///     Render the button with the given spriteBatch.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Render(SpriteBatch spriteBatch)
        {
            _texture.Render(spriteBatch);

            if (_hasText &&_textString != null)
            {
                // Do some work to center the text in the button.
                int maxWidth = Size.X - 8;
                string clippedString = Utility.TextHelper.ClipText(Font, _textString, maxWidth);
                Vector2 textSize = Font.MeasureString(clippedString);

                Point center = Bounds.Center;
                _text.RelativePosition = new Point((int)Math.Floor(center.X - textSize.X / 2), (int)Math.Floor(center.Y - textSize.Y / 2));
                _text.Text = clippedString;

                _text.Render(spriteBatch);
            }
        }

        /// <summary>
        ///     Update the button, making it check for click and hold events.
        /// </summary>
        public void Update()
        {
            // Non-focused buttons don't update.
            if (IsFocused)
            {
                InputHandler input = InputHandler.GetInstance();
                Point mousePos = Mouse.GetState().Position;

                if (ScreenArea.Contains(mousePos) && input.IsMouseReleased(MouseButton.Left))
                {
                    if (Click != null)
                    {
                        Click.Invoke(this, null);
                    }
                }
                else if (ScreenArea.Contains(mousePos) && input.IsMouseHold(MouseButton.Left))
                {
                    if (Hold != null)
                    {
                        Hold.Invoke(this, null);
                    }
                }
            }
        }
    }
}
