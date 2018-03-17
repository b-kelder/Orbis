using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     Text with a fixed position.
    /// </summary>
    public class RelativeText : RelativeElement, IRenderableElement
    {
        /// <summary>
        ///     The font to use for drawing the text.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public SpriteFont Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value ?? throw new ArgumentNullException();
            }
        }
        private SpriteFont _font;

        /// <summary>
        ///     The layer depth of the text.
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
                return _layerDepth;
            }
            set
            {
                // Range check between 0 and 1 (inclusive).
                _layerDepth = (value >= 0 && value <= 1) ? value : throw new ArgumentOutOfRangeException();
            }
        }
        private float _layerDepth;

        /// <summary>
        ///     Effects to use for drawing the text.
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; }

        /// <summary>
        ///     The color in which the text will be drawn.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        ///     The text to draw.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value ?? throw new ArgumentNullException();
            }
        }
        private string _text;

        /// <summary>
        ///     Create a new <see cref="RelativeText"/>.
        /// </summary>
        /// 
        /// <param name="font">
        ///     The font to use for drawing the text.
        /// </param>
        public RelativeText(IPositionedElement parent, SpriteFont font) : base(parent)
        {
            Font = font;
            Text = "";
            TextColor = Color.Black;
            LayerDepth = 0F;
            SpriteEffects = SpriteEffects.None;
        }

        /// <summary>
        ///     Get the size of the text.
        /// </summary>
        /// <returns></returns>
        public override Point Size
        {
            get
            {
                Vector2 measuredSize = Font.MeasureString(Text);
                return new Point((int)Math.Ceiling(measuredSize.X), (int)Math.Ceiling(measuredSize.Y));
            }
        }

        /// <summary>
        ///     Render the text using the given spriteBatch.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spriteBatch to use for drawing.
        /// </param>
        public void Render(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                spriteBatch.DrawString(Font,
                Text,
                new Vector2(Position.X, Position.Y),
                TextColor,
                0F,
                Vector2.Zero,
                1F,
                SpriteEffects,
                LayerDepth);
            }
        }
    }
}
