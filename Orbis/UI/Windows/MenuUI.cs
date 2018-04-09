using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.Engine;
using Orbis.States;
using Orbis.UI.Elements;
using Orbis.UI.Utility;
using System;

/// <Author>
/// Bram Kelder, Thomas Stoevelaar
/// </Author>
namespace Orbis.UI.Windows
{
    public class MenuUI : UIWindow
    {
        // Referance to the orbis class
        private Orbis orbis;
        // Main menu background and items
        private RelativeTexture menuBackground;
        private RelativeTexture orbisLogo;
        private Button openPopupButton;
        private Button settingsMenu;
        private Button quitButton;
        // Popup background and items
        private RelativeTexture popupBackground;
        private Button startButton;
        private RelativeText text;
        private InputNumberField seed;
        private InputNumberField civs;
        private InputNumberField radius;
        private InputNumberField ticks;

        private StateManager stateManager;

        /// <summary>
        /// Constructor for the menu UI
        /// </summary>
        /// <param name="game"></param>
        public MenuUI(Game game) : base(game)
        {
            orbis = (Orbis)game;
            stateManager = StateManager.GetInstance();
            SpriteFont font = _contentManager.GetFont("DebugFont");

            // Background for the menu
            AddChild(menuBackground = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(Color.LightGray), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopLeft,
                RelativePosition = new Point(0, 0),
                LayerDepth = 1,
            });

            // The orbis logo
            AddChild(orbisLogo = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetTexture("UI/Orbis-Icon"), new Rectangle(0, 0, 1520, 1520)))
            {
                Size = new Point(_game.Window.ClientBounds.Width / 4, _game.Window.ClientBounds.Width / 4),
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8, -(_game.Window.ClientBounds.Height / 2) + 50),
                LayerDepth = 0.5f
            });

            // Button to open the popup menu
            AddChild(openPopupButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Start"), new Rectangle(0, 0, 228, 64)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 6, _game.Window.ClientBounds.Height / 12),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 12, -_game.Window.ClientBounds.Height / 8 + 100),
                LayerDepth = 0.5f,
                Focused = true
            });

            // Button for settings menu
            AddChild(settingsMenu = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Settings"), new Rectangle(0, 0, 228, 64)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 6, _game.Window.ClientBounds.Height / 12),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 12, 0 + 100),
                LayerDepth = 0.5f,
                Focused = true
            });

            // Button for exiting the game
            AddChild(quitButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Quit"), new Rectangle(0, 0, 228, 64)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 6, _game.Window.ClientBounds.Height / 12),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 12, _game.Window.ClientBounds.Height / 8 + 100),
                LayerDepth = 0.5f,
                Focused = true
            });

            // Background for the popup menu
            AddChild(popupBackground = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(Color.DarkGray), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width / 3, _game.Window.ClientBounds.Height / 3),
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 6, 0),
                LayerDepth = 0.4f,
                Visible = false
            });

            // Text for the popup window
            AddChild(text = new RelativeText(this, font)
            {
                AnchorPosition = AnchorPosition.Center,
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 6 + 10, 10),
                Text = "Generate a world based on the following settings:\r\n\r\nSeed:\r\n\r\nCivilization count:\r\n\r\nMap radius:\r\n\r\nMonths to simulate:",
                Visible = false
            });

            // Get the length and height of text
            int textLength = (int)Math.Ceiling(font.MeasureString("Civilization count: ").X);
            int textHeight = (int)Math.Ceiling(font.MeasureString("A").Y);
            // Get the width of the max amount of input
            int fieldWidth = (int)Math.Ceiling(font.MeasureString("99999999").X);

            // Add the seed input field
            AddChild(seed = new InputNumberField(this)
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(fieldWidth + 2, font.LineSpacing + 2),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8 + 10 + textLength, 2 * textHeight + 5),
                MaxDigits = 8,
                Visible = false,
                LayerDepth = 0.03F
            });
            // Add the civ number input field
            AddChild(civs = new InputNumberField(this)
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(fieldWidth + 2, font.LineSpacing + 2),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8 + 10 + textLength, 4 * textHeight + 5),
                MaxDigits = 8,
                Visible = false,
                LayerDepth = 0.03F
            });
            // Add the radius input field
            AddChild(radius = new InputNumberField(this)
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(fieldWidth + 2, font.LineSpacing + 2),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8 + 10 + textLength, 6 * textHeight + 5),
                MaxDigits = 8,
                Visible = false,
                LayerDepth = 0.03F
            });
            // Add the ticks input field
            AddChild(ticks = new InputNumberField(this)
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(fieldWidth + 2, font.LineSpacing + 2),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 8 + 10 + textLength, 8 * textHeight + 5),
                MaxDigits = 8,
                Visible = false,
                LayerDepth = 0.03F
            });

            // Add text input events to the input fields
            game.Window.TextInput += seed.Window_TextInput;
            game.Window.TextInput += civs.Window_TextInput;
            game.Window.TextInput += radius.Window_TextInput;
            game.Window.TextInput += ticks.Window_TextInput;

            // Add startbutton to the popup menu
            AddChild(startButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Start"), new Rectangle(0, 0, 228, 64)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(_game.Window.ClientBounds.Width / 8, _game.Window.ClientBounds.Height / 16),
                RelativePosition = new Point(-_game.Window.ClientBounds.Width / 16, (int)(_game.Window.ClientBounds.Width / 7.5)),
                LayerDepth = 0.3f,
                Focused = false,
                Visible = false
            });

            // Add button click listeners
            startButton.Click += StartButton_Click;
            settingsMenu.Click += SettingsButton_Click;
            quitButton.Click += QuitButton_Click;
            openPopupButton.Click += PopupButton_Click;
        }

        /// <summary>
        /// Popup button click listener
        /// </summary>
        private void PopupButton_Click(object sender, EventArgs e)
        {
            popupBackground.Visible = true;
            startButton.Visible = true;
            text.Visible = true;

            seed.Visible = true;
            civs.Visible = true;
            radius.Visible = true;
            ticks.Visible = true;

            openPopupButton.Focused = false;
            settingsMenu.Focused = false;
            quitButton.Focused = false;
        }

        /// <summary>
        /// Quit button click listener
        /// </summary>
        private void QuitButton_Click(object sender, EventArgs e)
        {
            orbis.Exit();
        }

        /// <summary>
        /// Settings button click listener
        /// </summary>
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            orbis.UI.CurrentWindow = orbis.UI.SettingUI;
        }

        /// <summary>
        /// Start button click listener
        /// </summary>
        private void StartButton_Click(object sender, EventArgs e)
        {
            // Generate a new world :D
            orbis.GenerateWorld(seed.GetValue(), orbis.DecorationSettings, orbis.WorldSettings, orbis.BiomeCollection, orbis.CivSettings, civs.GetValue(), radius.GetValue(), ticks.GetValue());
            orbis.UI.CurrentWindow = orbis.UI.GameUI;
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

            if (popupBackground.Visible)
            {
                // Check if input is valid
                if (seed.GetValue() >= 0  && civs.GetValue() > 0 && radius.GetValue() > 0 && ticks.GetValue() >= 0)
                {
                    startButton.Focused = true;
                }
                else
                {
                    startButton.Focused = false;
                }
            }
        }

        /// <summary>
        ///     Event handler for window resizing.
        /// </summary>
        protected override void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            menuBackground.Size = new Point(_game.Window.ClientBounds.Width, _game.Window.ClientBounds.Height);
        }
    }
}
