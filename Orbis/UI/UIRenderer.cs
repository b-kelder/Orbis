using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Orbis.UI
{
    /// <summary>
    ///     A UI Window responsible for drawing the GUI and managing UI Elements.
    /// </summary>
    public class UIRenderer : DrawableGameComponent
    {
        // TEST
        public ProgressBar bar;

        /// <summary>
        ///     Gets the size of the window.
        /// </summary>
        private Point WindowSize
        {
            get;
            set;
        }

        /// <summary>
        ///     The root element for the window.
        /// </summary>
        public UIElement RootElement
        {
            get;
            set;
        }

        /// <summary>
        ///     The spritebatch used to draw the UI.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        ///     Represents the currently shown window.
        /// </summary>
        /// <param name="game"></param>
        public UIRenderer(Game game) : base(game)
        {
            RootElement = new Panel()
            {
                AnchorPosition = AnchorPosition.TopLeft,
                LayerDepth = 1.00F
            };
        }

        public override void Initialize()
        {
            WindowSize = Game.Window.ClientBounds.Size;
            RootElement.Size = WindowSize;

            Texture2D barBack = new Texture2D(Game.GraphicsDevice, 1, 1);
            barBack.SetData(new Color[] { Color.WhiteSmoke });
            Texture2D redRect = new Texture2D(Game.GraphicsDevice, 1, 1);
            redRect.SetData(new Color[] { Color.Blue });
            SpriteFont messageFont = Game.Content.Load<SpriteFont>("DebugFont");

            bar = new ProgressBar()
            {
                AnchorPosition = AnchorPosition.BottomLeft,
                BackgroundTexture = barBack,
                BarTexture = redRect,
                Message = "Simulating",
                MessageFont = messageFont,
                RelativeLocation = new Point(20, -70),
                Size = new Point(WindowSize.X - 40, 50)
            };

            RootElement.AddChild(bar);

            TextBox box = new TextBox()
            {
                AnchorPosition = AnchorPosition.TopRight,
                BackgroundTexture = barBack,
                TextFont = messageFont,
                RelativeLocation = new Point(-420, 20),
                Size = new Point(400, 200)
            };

            box.Text.AppendLine("Test line meme test");
            box.Text.AppendLine("Test line meme test meme test line meme line test.");
            box.Text.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");

            RootElement.AddChild(box);

            Button button = new Button()
            {
                AnchorPosition = AnchorPosition.Center,
                BackgroundTexture = barBack,
                Text = "Click Me!",
                TextFont = messageFont,
                RelativeLocation = new Point(-50, -25),
                Size = new Point(100, 50),
                OnClick = () =>
                {
                    if (box.Visible)
                    {
                        box.Visible = false;
                    }
                    else
                    {
                        box.Visible = true;
                    }
                }
            };

            RootElement.AddChild(button);


            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        ///     Peform the UI update for this frame.
        /// </summary>
        /// 
        /// <param name="gameTime">
        ///     The game loop's current game time.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            var newWindowSize = Game.Window.ClientBounds.Size;
            if (WindowSize != newWindowSize)
            {
                RootElement.Size = newWindowSize;
                WindowSize = newWindowSize;
            }

            RootElement.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Draw the UI on screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            RootElement.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
