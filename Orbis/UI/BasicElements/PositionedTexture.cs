using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     A texture with a fixed position and dimensions.
    /// </summary>
    public class PositionedTexture : IBasicElement
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
        public virtual float LayerDepth
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
        public virtual Point Position { get; set; }

        /// <summary>
        ///     The dimensions of the texture.
        /// </summary>
        public virtual Point Size { get; set; }

        /// <summary>
        ///     Effects to use for drawing the texture.
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; }

        /// <summary>
        ///     The texture that will be drawn.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public Texture2D Texture
        {
            get
            {
                return _Texture;
            }
            set
            {
                _Texture = value ?? throw new ArgumentNullException();
            }
        }
        private Texture2D _Texture;

        /// <summary>
        ///     Create a new <see cref="PositionedTexture"/>.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException" />
        public PositionedTexture(Texture2D texture)
        {
            Texture = texture;
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
        public virtual void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                Bounds,
                null,
                Color.White,
                0F,
                Vector2.Zero,
                SpriteEffects,
                LayerDepth);
        }
    }
}
