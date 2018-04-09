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
        private Button backButton;

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

            AddChild(backButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Back"), new Rectangle(0, 0, 228, 64)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 8,  _game.Window.ClientBounds.Height / 16),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 16, (int)(_game.Window.ClientBounds.Width / 7.5)),
                LayerDepth = 0.3f,
                Focused = true,
                Visible = true
            });

            backButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            orbis.UI.CurrentWindow = new MenuUI(orbis);
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
