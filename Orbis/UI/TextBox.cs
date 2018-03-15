using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orbis.UI
{
    /// <summary>
    ///     A text box in the Orbis UI.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class TextBox : UIElement
    {
        /// <summary>
        ///     Get the texbox's children (it has none).
        /// </summary>
        public override UIElement[] Children
        {
            get
            {
                return new UIElement[0];
            }
        }

        /// <summary>
        ///     The texture that will be used to draw the background.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        ///     The text displayed in the textbox.
        /// </summary>
        public string Text
        {
            get
            {
                string textString = (_stringBuilder != null) ? _stringBuilder.ToString() : "";
                return textString;   
            }
        }

        /// <summary>
        ///     A stringBuilder used for formatting text in the textbox.
        /// </summary>
        private StringBuilder _stringBuilder;

        /// <summary>
        ///     The lines in the textbox, does allow for "lines" to be multiline.
        /// </summary>
        public List<string> Lines { get; set; }

        /// <summary>
        ///     The font used for drawing the text.
        /// </summary>
        public SpriteFont TextFont { get; set; }

        /// <summary>
        ///     The color of the text.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        ///     The graphicsDevice used by this textbox.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; set; }

        /// <summary>
        ///     A texture that the full text of the textbox has been drawn to.
        /// </summary>
        private Texture2D _fulltext;

        /// <summary>
        ///     The viewport within the full text that is shown.
        /// </summary>
        private Rectangle _viewPort;

        /// <summary>
        ///     The scrollbar used to scroll in the text.
        /// </summary>
        public Scrollbar Scrollbar { get; set; }

        /// <summary>
        ///     Create a new <see cref="TextBox"/>.
        /// </summary>
        public TextBox()
        {
            _stringBuilder = new StringBuilder();
            _viewPort = new Rectangle(0, 0, Size.X - 25, Size.Y);
            Lines = new List<string>();
            TextColor = Color.Black;
            Scrollbar = new Scrollbar()
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativeLocation = new Point(-15, 0),
                Parent = this
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (Scrollbar.Visible)
            {
                _viewPort.Y = (int)Math.Floor((_fulltext.Height - _viewPort.Height) * (Scrollbar.HandlePosition / 100));
            }
            Scrollbar.Update(gameTime);
            // No base update needed; textboxes do not have children.
        }

        /// <summary>
        ///     Draw the textbox.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The SpriteBatch used for drawing the textbox.
        /// </param>
        /// 
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // There will be no drawing if the textbox is not visible.
            if (Visible && spriteBatch != null)
            {
                // Draw background only if it is set.
                if (BackgroundTexture != null)
                {
                    spriteBatch.Draw(BackgroundTexture,
                        AbsoluteRectangle,
                        null,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth);
                }

                // Only draw the text if there is any and if the font has been specified.
                if (TextFont != null && Lines.Count > 0 && _fulltext != null)
                {
                    Rectangle absoluteRect = new Rectangle(AbsoluteRectangle.X,
                        AbsoluteRectangle.Y,
                        AbsoluteRectangle.Width - 25,
                        AbsoluteRectangle.Height);

                    if (_viewPort.Height < absoluteRect.Height)
                    {
                        absoluteRect.Height = _viewPort.Height;
                    }

                    spriteBatch.Draw(_fulltext,
                        absoluteRect,
                        _viewPort,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth - 0.001F);

                    System.Diagnostics.Debug.WriteLine(Text);
                }

                if (Scrollbar != null && Scrollbar.Visible)
                {
                    Scrollbar.Draw(spriteBatch, gameTime);
                }
            }

            // No base Draw needed; textboxes do not have children.
        }

        /// <summary>
        ///     Textboxes can not have children; do not use.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public override void AddChild(UIElement child)
        {
            throw new OrbisUIException("Textboxes can not have children.");
        }

        /// <summary>
        ///     Textboxes can not have children; do not use.
        /// </summary>
        /// <exception cref="OrbisUIException" />
        public override void ReplaceChild(int childIndex, UIElement newChild)
        {
            throw new OrbisUIException("Textboxes can not have children.");
        }

        /// <summary>
        ///     Update the textbox's layout.
        /// </summary>
        public override void UpdateLayout()
        {
            // Don't bother processing when the required resources aren't set or when there are no lines.
            if (GraphicsDevice != null && TextFont != null && Lines.Count > 0 && Size.Y > 0)
            {
                WrapLines();
            }

            if (_fulltext != null)
            {
                if (Size.Y  > _fulltext.Height)
                {
                    if (Scrollbar.Visible)
                    {
                        Scrollbar.Visible = false;
                    }
                    _viewPort.Height = _fulltext.Height;
                }
                else
                {
                    if (!Scrollbar.Visible)
                    {
                        Scrollbar.Visible = true;
                    }
                    _viewPort.Height = Size.Y;
                }
                _viewPort.Width = Size.X - 25;
            }

            if (Scrollbar.Visible)
            {
                Scrollbar.Size = new Point(15, AbsoluteRectangle.Height);
                Scrollbar.LayerDepth = LayerDepth - 0.001F;
            }

            // No base updateLayout needed; textboxes do not have children.
        }

        /// <summary>
        ///     Wrap a the lines to fit within the text box.
        /// </summary>
        private void WrapLines()
        {
            List<string> wrappedLines = new List<string>();
            _stringBuilder = new StringBuilder();
            float spaceWidth = TextFont.MeasureString(" ").X;

            foreach (string line in Lines)
            {
                // The line is wrapped to fit within the text box.
                string wrappedLine = WrapLine(line, spaceWidth, Size.X - 25);
                wrappedLines.Add(_stringBuilder.ToString());
            }

            _stringBuilder.Clear();
            foreach (string wrappedLine in wrappedLines)
            {
                _stringBuilder.AppendLine(wrappedLine);
            }

            // The text is drawn to a RenderTarget2D so it can be scrolled through.
            Vector2 finalSize = TextFont.MeasureString(_stringBuilder);
            RenderTarget2D fulltextRenderTarget = new RenderTarget2D(GraphicsDevice,
                Size.X - 25,
                (int)Math.Ceiling(finalSize.Y));

            GraphicsDevice.SetRenderTarget(fulltextRenderTarget);
            SpriteBatch textBatch = new SpriteBatch(GraphicsDevice);

            textBatch.Begin();
            GraphicsDevice.Clear(Color.Transparent);
            textBatch.DrawString(TextFont, _stringBuilder, new Vector2(4, 4), TextColor);
            textBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            _fulltext = fulltextRenderTarget;
        }

        /// <summary>
        ///     Get a wrapped line that fits within the constraints of the text box.
        /// </summary>
        /// 
        /// <param name="line">
        ///     The line to wrap.
        /// </param>
        /// <param name="spaceWidth">
        ///     The width of a space in the font.
        /// </param>
        /// <param name="maxWidth">
        ///     The maximum line width.
        /// </param>
        /// 
        /// <returns>
        ///     The wrapped line.
        /// </returns>
        private string WrapLine(string line, float spaceWidth, float maxWidth)
        {
            float lineWidth = 0F;
            _stringBuilder.Clear();
            string[] words = line.Split(' ');
            foreach (string word in words)
            {
                float wordWidth = TextFont.MeasureString(word).X;

                if (Math.Ceiling(lineWidth + wordWidth) < maxWidth)
                {
                    _stringBuilder.Append(word + " ");
                    lineWidth += wordWidth + spaceWidth;
                }
                else
                {
                    if (Math.Ceiling(wordWidth) > maxWidth)
                    {
                        // Wrap words that don't fit in their entirety.
                        string remainder = "";
                        float remainderWidth;
                        do
                        {
                            float partWidth;
                            string part = word;
                            do
                            {
                                remainder = part.Substring(part.Length - 1) + remainder;
                                part = part.Substring(0, part.Length - 1);
                                partWidth = TextFont.MeasureString(part).X;
                            } while (Math.Ceiling(partWidth) > maxWidth);

                            _stringBuilder.Append(part + "\n");
                            remainderWidth = TextFont.MeasureString(remainder).X;

                        } while (Math.Ceiling(remainderWidth) > maxWidth);
                        _stringBuilder.Append(remainder + " ");
                    }
                    else
                    {
                        // Add the word on a new line.
                        _stringBuilder.Append("\n" + word + " ");
                        lineWidth = spaceWidth;
                    }
                }
            }

            return _stringBuilder.ToString();
        }

        /// <summary>
        ///     Append a line to the text in the textbox.
        /// </summary>
        /// 
        /// <param name="text">
        ///     The text to append to the textBox as a new line.
        /// </param>
        public void AppendLine(string text)
        {
            Lines.Add(text);

            UpdateLayout();
        }
    }
}
