using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        ///     Memes
        /// </summary>
        public void Tet()
        {
            var mouseState = Mouse.GetState();
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
