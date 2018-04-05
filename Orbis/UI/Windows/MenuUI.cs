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
    public class MenuUI : UIWindow
    {
        private Orbis orbis;
        private RelativeTexture background;
        private Button startButton;

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

            AddChild(startButton = new Button(this, new SpriteDefinition(contentManager.GetColorTexture(Color.Black), new Rectangle(0, 0, 1, 1)))
            {
                AnchorPosition = AnchorPosition.Center,
                Size = new Point(200, 50),
                RelativePosition = new Point(-100, 0),
                LayerDepth = 0,
                IsFocused = true
            });

            startButton.Click += StartButton_Click;
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
