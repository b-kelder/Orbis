using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Elements;
using Orbis.UI.Utility;
using System;

namespace Orbis.UI.Windows
{
    public class SettingsUI : UIWindow
    {
        private Orbis orbis;
        private RelativeTexture background;
        private InputNumberField numberField;

        private Color BACKGROUND_COLOR = Color.LightGray;

        public SettingsUI(Game game) : base(game)
        {
            orbis = (Orbis)game;

            // Background for UI
            AddChild(background = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(BACKGROUND_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopLeft,
                RelativePosition = new Point(0, 0),
                LayerDepth = 1
            });

            SpriteFont font = _contentManager.GetFont("DebugFont");
            int fieldWidth = (int)Math.Ceiling(font.MeasureString("99999999").X);
            
            AddChild(numberField = new InputNumberField(this)
            {
                Size = new Point(fieldWidth + 2, font.LineSpacing + 2),
                RelativePosition = new Point(200, 200),
                Focused = true,
                MaxDigits = 8,
                Visible = true,
                LayerDepth = 0.09F
            });

            game.Window.TextInput += numberField.Window_TextInput;
        }
        /// <summary>
        ///     Draw the TestWindow.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        ///     Event handler for window resizing.
        /// </summary>
        protected override void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            background.Size = new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height);
        }
    }
}
