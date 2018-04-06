using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.Engine;
using Orbis.UI.Utility;

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
                if (!_renderText.Text.Contains("_"))
                {
                    _renderText.Text += "_";
                }
            }
        }
        private bool _focused;

        public void Render(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                _background.Render(spriteBatch);
                _renderText.Render(spriteBatch);
            }
        }

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

                _renderText.Text = _textSb.ToString();
                if (_textSb.Length != MaxDigits)
                {
                    _renderText.Text += "_";
                }
            }
            else if (_renderText.Text.EndsWith("_"))
            {
                _renderText.Text = _renderText.Text.TrimEnd('_');
            }
        }

        /// <summary>
        ///     Update the button, making it check for click and hold events.
        /// </summary>
        public void Update()
        {
            // Non-focused buttons don't update.
            InputHandler input = InputHandler.GetInstance();
            Point mousePos = input.GetMousePosition();

            if (Bounds.Contains(mousePos))
            {
                Focused = true;
            }
            else
            {
                Focused = false;
            }
        }
    }
}
