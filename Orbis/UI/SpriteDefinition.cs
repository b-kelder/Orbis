using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI
{
    /// <summary>
    ///     The definition of a sprite
    /// </summary>
    public struct SpriteDefinition
    {
        /// <summary>
        ///     The spritesheet containing the sprite.
        /// </summary>
        public Texture2D SpriteSheet { get; set; }

        /// <summary>
        ///     The position on the spritesheet that contains the sprite.
        /// </summary>
        public Rectangle SourceRectangle { get; set; }

        /// <summary>
        ///     Create a new <see cref="SpriteDefinition"/> with the given values.
        /// </summary>
        /// 
        /// <param name="spriteSheet">
        ///     The spritesheet containing the sprite.
        /// </param>
        /// <param name="sourceRectangle">
        ///     The position on the spritesheet that contains the sprite.
        /// </param>
        public SpriteDefinition(Texture2D spriteSheet, Rectangle sourceRectangle)
        {
            SpriteSheet = spriteSheet;
            SourceRectangle = sourceRectangle;
        }
    }
}
