using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orbis.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Orbis.UI
{
    public class Button : Panel
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
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (this.AbsoluteRectangle.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    System.Diagnostics.Debug.WriteLine("kek");
                }
            }
        }
    }
}
