using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orbis.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orbis.UI
{
    public class Button : UIElement
    {
        /// <summary>
        ///     Get the button's children.
        /// </summary>
        public override UIElement[] Children
        {
            get
            {
                return new UIElement[0];
            }
        }
        
        /// <summary>
        ///     The background texture to use for drawing the button.
        /// </summary>
        public Texture2D BackgroundTexture { get; set; }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
