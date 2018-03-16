using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.UI.BasicElements;
using Orbis.UI.Utility;
using Microsoft.Xna.Framework.Graphics;

namespace Orbis.UI
{
    /// <summary>
    ///     The Orbis main menu buttons.
    /// </summary>
    public class MainMenu : UIElement
    {
        private Button _startButton;

        public MainMenu(Game game) : base(game)
        {
            AnchorPosition = AnchorPosition.Center;

            if (BasicTextureFactory.TryGetInstance(out BasicTextureFactory textureFactory)
                && UIContentManager.TryGetInstance(out UIContentManager contentManager))
            {
                SpriteFont font = contentManager.GetFont("DebugFont");
                Texture2D buttonTexture = textureFactory.CreateBasicTexture(Color.WhiteSmoke);
                SpriteDefinition buttonDef = new SpriteDefinition(buttonTexture, new Rectangle(0, 0, 1, 1));
                _startButton = new Button(buttonDef, font)
                {
                    LayerDepth = 0.9F,
                    Position = Point.Zero,
                    Size = new Point(400, 50),
                    IsFocused = true
                };

            }
        }
    }
}
