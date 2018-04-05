using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Utility;
using Orbis.UI.Elements;

namespace Orbis.UI.Windows
{
    public class MenuUI : UIWindow
    {
        private Orbis orbis;
        private RelativeTexture background;
        private RelativeTexture backgroundPopup;
        private RelativeTexture logo;
        private Button popupButton;
        private Button startButton;
        private Button optionsButton;
        private Button quitButton;

        private Color BACKGROUND_COLOR = Color.LightGray;

        public MenuUI(Game game) : base(game)
        {
            orbis = (Orbis)game;
            
            UIContentManager.TryGetInstance(out UIContentManager contentManager);

            // Background for UI
            AddChild(background = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(BACKGROUND_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopLeft,
                RelativePosition = new Point(0, 0),
                LayerDepth = 1
            });

            AddChild(logo = new RelativeTexture(this, new SpriteDefinition(contentManager.GetTexture("UI/Orbis-Icon"), new Rectangle(0, 0, 1520, 1520)))
            {
                Size = new Point(500, 500),
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-250, -(_game.Window.ClientBounds.Height / 2) + 100),
                LayerDepth = 0.5f
            });

            AddChild(popupButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Blue), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(400, 100),
                RelativePosition = new Point(-200, -150 + 100),
                LayerDepth = 0.5f,
                IsFocused = true
            });

            AddChild(optionsButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Orange), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(400, 100),
                RelativePosition = new Point(-200, 0 + 100),
                LayerDepth = 0.5f,
                IsFocused = true
            });

            AddChild(quitButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Yellow), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(400, 100),
                RelativePosition = new Point(-200, 150 + 100),
                LayerDepth = 0.5f,
                IsFocused = true
            });

            AddChild(backgroundPopup = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Green), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width / 4, _game.Window.ClientBounds.Height / 4),
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8, 0),
                LayerDepth = 0.4f,
                Visible = false
            });

            AddChild(startButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Purple), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 8, _game.Window.ClientBounds.Height / 16),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 16, _game.Window.ClientBounds.Width / 8 - _game.Window.ClientBounds.Height / 16),
                LayerDepth = 0.3f,
                IsFocused = false,
                Visible = false
            });

            startButton.Click += StartButton_Click;
            optionsButton.Click += OptionsButton_Click;
            quitButton.Click += QuitButton_Click;
            popupButton.Click += PopupButton_Click;
        }

        private void PopupButton_Click(object sender, EventArgs e)
        {
            backgroundPopup.Visible = true;
            startButton.Visible = true;
            startButton.IsFocused = true;

            popupButton.IsFocused = false;
            optionsButton.IsFocused = false;
            quitButton.IsFocused = false;
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            orbis.Exit();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            orbis.UI.CurrentWindow = new GameUI(orbis);
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
