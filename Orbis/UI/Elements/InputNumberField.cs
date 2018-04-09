using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.Engine;
using Orbis.UI.Utility;
using System;
using System.Text;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     An input field in the Orbis UI that the user can enter numbers into.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class InputNumberField : RelativeElement, IRenderableElement, IUpdateableElement
    {
        private RelativeTexture _background;
        private RelativeText _renderText;
        private StringBuilder _textSb;

        /// <summary>
        ///     The maximum amount of digits that can be entered in the field.
        /// </summary>
        public int MaxDigits { get; set; }

        /// <summary>
        ///     The size of the input field.
        /// </summary>
        public override Point Size
        {
            get
            {
                return _background.Size;
            }
            set
            {
                _background.Size = value;
            }
        }

        /// <summary>
        ///     The layer depth of the input field.
        /// </summary>
        /// 
        /// <remarks>
        ///     Value should be between 0 and 1, with zero being the front and 1 being the back.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException" />
        public float LayerDepth
        {
            get
            {
                return _background.LayerDepth;
            }
            set
            {
                _background.LayerDepth = value;
                _renderText.LayerDepth = value - 0.01F;
            }
        }

        /// <summary>
        ///     Is the input field visible?
        /// </summary>
        public bool Visible
        {
            get
            {
                return _background.Visible;
            }
            set
            {
                _background.Visible = value;
                _renderText.Visible = value;
            }
        }

        /// <summary>
        ///     Is the input field in focus?.
        /// </summary>
        public bool Focused
        {
            get
            {
                return _focused;
            }
            set
            {
                _focused = value;

                if (value && _textSb.Length < MaxDigits && !_renderText.Text.EndsWith("_"))
                {
                    _renderText.Text += "_";
                }
                else if (_renderText.Text.EndsWith("_"))
                {
                    _renderText.Text = _renderText.Text.TrimEnd('_');
                }
            }
        }
        private bool _focused;

        /// <summary>
        ///     Create a new <see cref="InputNumberField"/>.
        ///     Creates sub elements and sets default values.
        /// </summary>
        /// <param name="parent"></param>
        public InputNumberField(IPositionedElement parent) : base(parent)
        {
            if (UIContentManager.TryGetInstance(out UIContentManager contentManager))
            {
                _background = new RelativeTexture(this,
                    new SpriteDefinition(contentManager.GetColorTexture(Color.White), new Rectangle(Point.Zero, new Point(1, 1))))
                {
                    AnchorPosition = AnchorPosition.TopLeft,
                    RelativePosition = Point.Zero,
                };

                _renderText = new RelativeText(this, contentManager.GetFont("DebugFont"))
                {
                    AnchorPosition = AnchorPosition.TopLeft,
                    RelativePosition = new Point(1, 1)
                };

                _textSb = new StringBuilder();

                Visible = true;
            }
            else
            {
                throw new InvalidOperationException("UI content manager does not exist.");
            }
        }

        public int GetValue()
        {
            return (Int32.TryParse(_textSb.ToString().TrimEnd('_'), out int result)) ? result : -1; // return the result or -1 if it can't be parsed.
        }

        public void SetValue(int value)
        {
            _textSb.Clear().Append(value);
        }
        
        /// <summary>
        ///     Event handler function for text input.
        ///     Attach to the window's TextInput event to take buffered text input.
        /// </summary>
        public void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (Focused)
            {
                Keys inputKey = e.Key;
                char inputCharacter = e.Character;
                if (Char.IsDigit(inputCharacter) && _textSb.Length < MaxDigits)
                {
                    _textSb.Append(inputCharacter);
                }
                else if (inputCharacter == '\b')
                {
                    if (_textSb.Length > 0)
                    {
                        _textSb.Remove(_textSb.Length - 1, 1);
                    }
                }

                Refresh();
            }
        }

        /// <summary>
        ///     Update the input field.
        ///     Checks for clicks on the field to set focus for input.
        /// </summary>
        public void Update()
        {
            InputHandler input = InputHandler.GetInstance();
            Point mousePos = input.GetMousePosition();
            bool clicked = input.IsMouseReleased(MouseButton.Left);

            if (Bounds.Contains(mousePos))
            {
                if (clicked)
                {
                    Focused = true;
                }
            }
            else if (clicked)
            {
                Focused = false;
            }
        }

        /// <summary>
        ///     Render the input number field.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spritebatch used for rendering.
        /// </param>
        public void Render(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                _background.Render(spriteBatch);
                _renderText.Render(spriteBatch);
            }
        }

        public void Refresh()
        {
            _renderText.Text = _textSb.ToString();
            if (Focused && _textSb.Length < MaxDigits)
            {
                _renderText.Text += "_";
            }
        }
    }
}
