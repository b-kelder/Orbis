using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.Engine;
using Orbis.States;
using Orbis.UI.Elements;
using Orbis.UI.Utility;
using System;

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
        private StateManager stateManager;

        private Color BACKGROUND_COLOR = Color.LightGray;
        private RelativeText text;

        public MenuUI(Game game) : base(game)
        {
            orbis = (Orbis)game;
            stateManager = StateManager.GetInstance();

            // Background for UI
            AddChild(background = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(BACKGROUND_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopLeft,
                RelativePosition = new Point(0, 0),
                LayerDepth = 1,
            });

            AddChild(logo = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetTexture("UI/Orbis-Icon"), new Rectangle(0, 0, 1520, 1520)))
            {
                Size = new Point(_game.Window.ClientBounds.Width / 4, _game.Window.ClientBounds.Width / 4),
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8, -(_game.Window.ClientBounds.Height / 2) + 50),
                LayerDepth = 0.5f
            });

            AddChild(popupButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Start"), new Rectangle(0, 0, 200, 57)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 6, _game.Window.ClientBounds.Height / 12),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 12, -_game.Window.ClientBounds.Height / 8 + 100),
                LayerDepth = 0.5f,
                Focused = true
            });

            AddChild(optionsButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Settings"), new Rectangle(0, 0, 200, 57)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 6, _game.Window.ClientBounds.Height / 12),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 12, 0 + 100),
                LayerDepth = 0.5f,
                Focused = true
            });

            AddChild(quitButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Quit"), new Rectangle(0, 0, 200, 57)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 6, _game.Window.ClientBounds.Height / 12),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 12, _game.Window.ClientBounds.Height / 8 + 100),
                LayerDepth = 0.5f,
                Focused = true
            });

            AddChild(backgroundPopup = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(Color.DarkGray), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width / 4, _game.Window.ClientBounds.Height / 4),
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8, 0),
                LayerDepth = 0.4f,
                Visible = false
            });

            AddChild(text = new RelativeText(this, _contentManager.GetFont("DebugFont"))
            {
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8 + 10, 10),
                Text = "Generate a world based on the following settings:\r\n" +
                "Seed: " + Orbis.TEST_SEED + 
                "\r\nCivilization count: " + Orbis.TEST_CIVS +
                "\r\nMap radius: " + Orbis.TEST_RADIUS +
                "\r\nMonths to simulate: " + Orbis.TEST_TICKS,
                Visible = false
            });

            AddChild(startButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Start"), new Rectangle(0, 0, 200, 57)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 8, _game.Window.ClientBounds.Height / 16),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 16, _game.Window.ClientBounds.Width / 8 - _game.Window.ClientBounds.Height / 16),
                LayerDepth = 0.3f,
                Focused = false,
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
            startButton.Focused = true;
            text.Visible = true;

            popupButton.Focused = false;
            optionsButton.Focused = false;
            quitButton.Focused = false;
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            orbis.Exit();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            orbis.UI.CurrentWindow = new SettingsUI(orbis);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            orbis.GenerateWorld(Orbis.TEST_SEED, orbis.DecorationSettings, orbis.WorldSettings, orbis.BiomeCollection, orbis.CivSettings, Orbis.TEST_CIVS, Orbis.TEST_RADIUS, Orbis.TEST_TICKS);
            orbis.UI.CurrentWindow = new GameUI(orbis);
            stateManager.SetActiveState(StateManager.State.GAME);
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
