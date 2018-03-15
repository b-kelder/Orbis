using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     Text with a fixed position.
    /// </summary>
    public class PositionedText : IBasicElement
    {
        /// <summary>
        ///     The position of the text;
        /// </summary>
        public Point Position
        {
            get
            {
                return new Point((int)_position.X, (int)_position.Y);
            }
            set
            {
                _position = new Vector2(value.X, value.Y);
            }
        }
        private Vector2 _position;

        /// <summary>
        ///     The combination of position and size for the text.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                Vector2 measuredSize = Font.MeasureString(Text);
                Point size = new Point((int)Math.Ceiling(measuredSize.X), (int)Math.Ceiling(measuredSize.Y));
                return new Rectangle(Position, size);
            }
        }

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
        ///     The color in which the text will be drawn.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        ///     Create a new <see cref="PositionedText"/>.
        /// </summary>
        /// 
        /// <param name="font">
        ///     The font to use for drawing the text.
        /// </param>
        public PositionedText(SpriteFont font)
        {
            Font = font;
            Text = "";
            TextColor = Color.Black;
            LayerDepth = 0F;
            Position = Point.Zero;
            SpriteEffects = SpriteEffects.None;
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
            spriteBatch.DrawString(Font,
                Text,
                _position,
                TextColor,
                0F,
                Vector2.Zero,
                1F,
                SpriteEffects,
                LayerDepth);
        }
    }
}
