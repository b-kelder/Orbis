using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     A texture with a fixed position and dimensions.
    /// </summary>
    public class PositionedTexture : IRenderableElement
    {
        /// <summary>
        ///     The combination of position and size for the texture.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position, Size);
            }
        }

        /// <summary>
        ///     The layer depth of the texture.
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
        ///     The position of the texture.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        ///     The dimensions of the texture.
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        ///     Effects to use for drawing the texture.
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; }

        /// <summary>
        ///     The sprite used for drawing the texture.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public SpriteDefinition SpriteDefinition
        {
            get
            {
                return _spriteDef;
            }
            set
            {
                // SpriteDefinitions without sprites are not allowed.
                _spriteDef = (value.SpriteSheet != null) ? value : throw new ArgumentNullException();
            }
        }
        private SpriteDefinition _spriteDef;

        /// <summary>
        ///     Create a new <see cref="PositionedTexture"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public PositionedTexture(SpriteDefinition spriteDef)
        {
            SpriteDefinition = spriteDef;
            LayerDepth = 0F;
            Position = Point.Zero;
            Size = Point.Zero;
            SpriteEffects = SpriteEffects.None;
        }

        /// <summary>
        ///     Render the texture using the given spriteBatch.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spritebatch to use for drawing.
        /// </param>
        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpriteDefinition.SpriteSheet,
                Bounds,
                SpriteDefinition.SourceRectangle,
                Color.White,
                0F,
                Vector2.Zero,
                SpriteEffects,
                LayerDepth);
        }
    }
}
