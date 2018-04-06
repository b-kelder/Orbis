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
        private RelativeTexture cellInfoBackground;
        private RelativeTexture backgroundProgressBar;
        private RelativeTexture scene;
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

        private bool screenResized;

        private int RIGHT_UI_WIDTH = 270;
        private int BOTTOM_UI_HEIGHT = 80;
        private Color UI_COLOR = Color.LightGray;

        public GameUI(Game game) : base(game)
        {
            orbis = (Orbis)game;
            logger = Logger.GetInstance();
            logExporter = new LogExporter();

            play = new SpriteDefinition(_contentManager.GetTexture("UI/Button_Play"), new Rectangle(0, 0, 96, 64));
            pause = new SpriteDefinition(_contentManager.GetTexture("UI/Button_Pause"), new Rectangle(0, 0, 96, 64));

            AddChild(playButton = new Button(this, play)
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-(RIGHT_UI_WIDTH - 96) / 2 - 96 , -10),
                Size = new Point(96, 64),
                LayerDepth = 0,
                Focused = true
            });

            AddChild(nextButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Next"), new Rectangle(0, 0, 70, 64)))
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-(RIGHT_UI_WIDTH - playButton.Size.X) / 2, -10),
                Size = new Point(70, 64),
                LayerDepth = 0,
                Focused = true
            });

            AddChild(exportButton = new Button(this, new SpriteDefinition(_contentManager.GetTexture("UI/Button_Export"), new Rectangle(0, 0, 70, 64)))
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-(RIGHT_UI_WIDTH - playButton.Size.X) / 2 - playButton.Size.X - 70, -10),
                Size = new Point(70, 64),
                LayerDepth = 0,
                Focused = true
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
            AddChild(backgroundProgressBar = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(UI_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH, BOTTOM_UI_HEIGHT),
                AnchorPosition = AnchorPosition.BottomLeft,
                RelativePosition = new Point(0,-BOTTOM_UI_HEIGHT),
                LayerDepth = 1
            });

            // Background for UI
            AddChild(background = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(UI_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(RIGHT_UI_WIDTH, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH, 0),
                LayerDepth = 1
            });

            // Background for UI
            AddChild(cellInfoBackground = new RelativeTexture(this, new SpriteDefinition(_contentManager.GetColorTexture(UI_COLOR), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(RIGHT_UI_WIDTH, 300),
                AnchorPosition = AnchorPosition.BottomRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH * 2, -BOTTOM_UI_HEIGHT - 300),
                LayerDepth = 0.5f
            });

            AddChild(text = new RelativeText(this, _contentManager.GetFont("DebugFont"))
            {
                Text = "Kill me pls",
                AnchorPosition = AnchorPosition.BottomRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH * 2, -BOTTOM_UI_HEIGHT - 300),
                LayerDepth = 0.4f
            });

            // Scene panel
            var sceneSize = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH, _game.Window.ClientBounds.Height - BOTTOM_UI_HEIGHT);
            AddChild(scene = new RelativeTexture(this, new SpriteDefinition(
                new RenderTarget2D(orbis.GraphicsDevice, sceneSize.X, sceneSize.Y),
                new Rectangle(0, 0, sceneSize.X, sceneSize.Y)))
            {
                Size = sceneSize,
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(0, 0),
                LayerDepth = 0.5f
            });
            // Scene panel itself is invisible, we just use it for size and texture storage
            scene.Visible = false;

            AddChild(civPanel = new CivPanel(this, orbis.Scene.Civilizations)
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH, 64),
                Size = new Point(RIGHT_UI_WIDTH, game.Window.ClientBounds.Height - 64),
                LayerDepth = 0.99F
            });

            screenResized = true;
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
            // Handle the scene seperately because for some reason it doesn't want to draw when used as a regular child
            spriteBatch.Draw(scene.SpriteDefinition.SpriteSheet, scene.SpriteDefinition.SourceRectangle, Color.White);
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
            scene.Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH,
                _game.Window.ClientBounds.Height - BOTTOM_UI_HEIGHT);
            screenResized = true;
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update()
        {
            if (screenResized)
            {
                // Update RenderTarget
                if(scene.SpriteDefinition.SpriteSheet != null)
                {
                    scene.SpriteDefinition.SpriteSheet.Dispose();
                }
                var rt = new RenderTarget2D(orbis.GraphicsDevice,
                    scene.Size.X, scene.Size.Y,
                    false, 
                    orbis.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                scene.SpriteDefinition = new SpriteDefinition(
                    rt,
                    new Rectangle(Point.Zero, scene.Size));
                orbis.SceneRenderer.RenderTarget = rt;
                screenResized = false;
            }

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
