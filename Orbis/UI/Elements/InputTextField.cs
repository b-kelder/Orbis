using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbis.UI.Utility;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     A text field in the Orbis UI that the user can enter text into.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    /// TODO: Implement new version.
    public class InputTextField : RelativeElement, IRenderableElement
    {
        public float LayerDepth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Visible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public InputTextField(IPositionedElement parent) : base(parent)
        {
            if (UIContentManager.TryGetInstance(out UIContentManager contentManager))
            {

            }
            else
            {
                throw new InvalidOperationException("UI content manager does not exist.");
            }
        }
    }
}
