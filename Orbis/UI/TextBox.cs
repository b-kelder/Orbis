using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        public StringBuilder Text
        {
            get;
            set;
        }

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
        ///     Create a new <see cref="TextBox"/>.
        /// </summary>
        public TextBox()
        {
            Text = new StringBuilder();
            TextColor = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {
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
                        0.00F,
                        Vector2.Zero,
                        SpriteEffects.None,
                        LayerDepth);
                }

                if (TextFont != null && !string.IsNullOrWhiteSpace(Text.ToString()))
                {
                    spriteBatch.DrawString(TextFont,
                        Text,
                        new Vector2(AbsoluteRectangle.Left + 2, AbsoluteRectangle.Top + 2),
                        TextColor,
                        0.00F,
                        Vector2.Zero,
                        1.00F,
                        SpriteEffects.None,
                        LayerDepth - 0.001F);
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
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder lineBuilder = new StringBuilder();
            float spaceWidth = TextFont.MeasureString(" ").X;

            string[] lines = Text.ToString().Split('\n');
            foreach (string line in lines)
            {
                lineBuilder.Clear();
                float currentLineWidth = 0.00F;
                string[] words = line.ToString().Split(' ');
                foreach (string word in words)
                {
                    float width = TextFont.MeasureString(word).X;

                    if (Math.Ceiling(currentLineWidth + width) < AbsoluteRectangle.Width - 4)
                    {
                        lineBuilder.Append(word + " ");
                        currentLineWidth += width + spaceWidth;
                    }
                    else
                    {
                        lineBuilder.Append("\n" + word + " ");
                        currentLineWidth = width + spaceWidth;
                    }
                }
                stringBuilder.Append(lineBuilder.ToString());
            }

            Vector2 finalSize = TextFont.MeasureString(stringBuilder);

            RenderTarget2D fullText = new RenderTarget2D(GraphicsDevice, (int)Math.Ceiling(finalSize.X), (int)Math.Ceiling(finalSize.Y));

            GraphicsDevice.SetRenderTarget(fullText);
            RasterizerState originalRasterizerState = GraphicsDevice.RasterizerState;
            BlendState originalBlendState = GraphicsDevice.BlendState;
            DepthStencilState originalDepthStencilState = GraphicsDevice.DepthStencilState;

            SpriteBatch fulltextSpriteBatch = new SpriteBatch(GraphicsDevice);
            fulltextSpriteBatch.Begin(SpriteSortMode.Deferred);
            fulltextSpriteBatch.DrawString(TextFont,
                stringBuilder,
                Vector2.Zero,
                TextColor);


        }
    }
}
