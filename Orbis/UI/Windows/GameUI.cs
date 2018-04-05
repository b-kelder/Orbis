using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Utility;

using Orbis.UI.Elements;
using Orbis.Simulation;
using Orbis.Events;
using Orbis.Events.Exporters;

namespace Orbis.UI.Windows
{
    public class GameUI : UIWindow
    {
        private ProgressBar progressBar;
        private RelativeTexture background;
        private RelativeTexture backgroundProgressBar;
        private RelativeText text;
        private Button playButton;
        private Button nextButton;
        private Button exportButton;
        private CivPanel civPanel;
        private Logger logger;
        private LogExporter logExporter;

        SpriteDefinition play;
        SpriteDefinition pause;

        private Orbis orbis;

        private int RIGHT_UI_WIDTH = 250;
        private Color UI_COLOR = Color.LightGray;

        public GameUI(Game game) : base(game)
        {
            orbis = (Orbis)game;
            logger = Logger.GetInstance();
            logExporter = new LogExporter();

            UIContentManager.TryGetInstance(out UIContentManager contentManager);

            play = new SpriteDefinition(contentManager.GetTexture("UI/Button_Play"), new Rectangle(0, 0, 96, 64));
            pause = new SpriteDefinition(contentManager.GetTexture("UI/Button_Pause"), new Rectangle(0, 0, 96, 64));

            AddChild(playButton = new Button(this, play)
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-(RIGHT_UI_WIDTH - 96) / 2 - 96 , -10),
                Size = new Point(96, 64),
                LayerDepth = 0,
                IsFocused = true
            });

            AddChild(nextButton = new Button(this, new SpriteDefinition(contentManager.GetTexture("UI/Button_Next"), new Rectangle(0, 0, 70, 64)))
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-(RIGHT_UI_WIDTH - playButton.Size.X) / 2, -10),
                Size = new Point(70, 64),
                LayerDepth = 0,
                IsFocused = true
            });

            AddChild(exportButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Orange), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-(RIGHT_UI_WIDTH - playButton.Size.X) / 2 - playButton.Size.X - 70, -10),
                Size = new Point(70, 64),
                LayerDepth = 0,
                IsFocused = true
            });

            playButton.Click += PlayButton_Click;
            nextButton.Click += NextButton_Click;
            exportButton.Click += ExportButton_Click;

            // Progress bar
            AddChild(progressBar = new ProgressBar(this)
            {
                AnchorPosition = AnchorPosition.BottomLeft,
                RelativePosition = new Point(20, -70),
                Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH - 40, 50),
                LayerDepth = 0
            });

            // Background for progressbar
            AddChild(backgroundProgressBar = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(UI_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH, 80),
                AnchorPosition = AnchorPosition.BottomLeft,
                RelativePosition = new Point(0,-80),
                LayerDepth = 1
            });

            // Background for UI
            AddChild(background = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(UI_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(RIGHT_UI_WIDTH, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH, 0),
                LayerDepth = 1
            });

            AddChild(civPanel = new CivPanel(this, orbis.Scene.Civilizations)
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH, 64),
                Size = new Point(RIGHT_UI_WIDTH, game.Window.ClientBounds.Height - 64),
                LayerDepth = 0.99F
            });
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            orbis.Simulator.SimulateOneTick();
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            orbis.Simulator.TogglePause();
        }

        /// <summary>
        /// Button to control exporting of logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (!orbis.Simulator.IsPaused())
            {
                orbis.Simulator.TogglePause();
            }
            //Create writer, add console exporter, export to console
            logExporter.Export(Logger.GetInstance().GetLog());
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
        ///     Event handler for window resizing.
        /// </summary>
        protected override void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            progressBar.Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH - 40, 50);
            background.Size = new Point(RIGHT_UI_WIDTH, _game.Window.ClientBounds.Height);
            backgroundProgressBar.Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH, 80);
            civPanel.Size = new Point(RIGHT_UI_WIDTH, _game.Window.ClientBounds.Height - 64);
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update()
        {
            if (orbis.Simulator.IsPaused())
            {
                playButton.SpriteDefinition = play;
            }
            else
            {
                playButton.SpriteDefinition = pause;
            }

            progressBar.Progress = ((float)orbis.Simulator.CurrentTick / Orbis.TEST_TICKS);
            progressBar.Message = "Date: " + orbis.Simulator.Date.ToString("MMM yyyy");

            base.Update();
        }
    }
}
