using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Utility;

using Orbis.UI.Elements;
using Orbis.Simulation;

namespace Orbis.UI.Windows
{
    public class GameUI : UIWindow
    {
        private ProgressBar progressBar;
        private RelativeTexture background;
        private RelativeTexture backgroundProgressBar;
        private RelativeText text;
        private Button playButton;

        private Orbis orbis;

        private int RIGHT_UI_WIDTH = 400;

        public GameUI(Game game) : base(game)
        {
            orbis = (Orbis)game;

            UIContentManager.TryGetInstance(out UIContentManager contentManager);

            AddChild(playButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.White), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-350, -10),
                Size = new Point(100, 50),
                LayerDepth = 1,
                IsFocused = true
            });

            playButton.Click += PlayButton_Click;

            // Progress bar
            AddChild(progressBar = new ProgressBar(this)
            {
                AnchorPosition = AnchorPosition.BottomLeft,
                RelativePosition = new Point(20, -70),
                Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH - 40, 50),
                LayerDepth = 0
            });

            // Background for progressbar
            AddChild(backgroundProgressBar = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(Color.White), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH, 80),
                AnchorPosition = AnchorPosition.BottomLeft,
                RelativePosition = new Point(0,-80),
                LayerDepth = 1
            });

            // Background for UI
            AddChild(background = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(Color.White), new Rectangle(0, 0, 1, 1)))
            {
                Size = new Point(400, _game.Window.ClientBounds.Height),
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH, 0),
                LayerDepth = 1
            });

            // Text for all civ data
            AddChild(text = new RelativeText(this, contentManager.GetFont("DebugFont"))
            {
                TextColor = Color.Black,
                Text = "",
                AnchorPosition = AnchorPosition.TopRight,
                RelativePosition = new Point(-RIGHT_UI_WIDTH + 50, 50),
                LayerDepth = 0
            });

            int x = 75;
            foreach (Civilization civ in orbis.Scene.Civilizations)
            {
                // Civ color
                AddChild(background = new RelativeTexture(this, new SpriteDefinition(contentManager.GetColorTexture(civ.Color), new Rectangle(0, 0, 1, 1)))
                {
                    Size = new Point(5, 80),
                    AnchorPosition = AnchorPosition.TopRight,
                    RelativePosition = new Point(-RIGHT_UI_WIDTH + 55, x),
                    LayerDepth = 0
                });
                x += 126;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (orbis.Simulator.IsPaused())
            {
                playButton.Text = "Pause";
            }
            else
            {
                playButton.Text = "Play";
            }
            
            orbis.Simulator.TogglePause();
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
            background.Size = new Point(400, _game.Window.ClientBounds.Height);
            backgroundProgressBar.Size = new Point(_game.Window.ClientBounds.Width - RIGHT_UI_WIDTH, 80);
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update()
        {
            progressBar.Progress = ((float)orbis.Simulator.CurrentTick / Orbis.TEST_TICKS);
            progressBar.Message = "Date: " + orbis.Simulator.Date.ToString("MMM yyyy");

            StringBuilder sb = new StringBuilder();

            foreach (var civ in orbis.Scene.Civilizations)
            {
                sb.AppendLine(civ.Name);
                sb.AppendLine("      Size: " + civ.Territory.Count);
                sb.AppendLine("      Population: " + civ.Population);
                sb.AppendLine("      Is Alive: " + civ.IsAlive);
                sb.AppendLine("      Wealth: " + civ.TotalWealth);
                sb.AppendLine("      Resource: " + civ.TotalResource);
                sb.AppendLine();
            }
            sb.AppendLine("\r\n\r\n");

            text.Text = sb.ToString();

            base.Update();
        }
    }
}
