﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.BasicElements;
using Orbis.UI.Utility;

namespace Orbis.UI
{
    /// <summary>
    ///     A UI Window responsible for drawing the GUI and managing UI Elements.
    /// </summary>
    public class UIRenderer : DrawableGameComponent
    {
        private Scrollbar test;

        /// <summary>
        ///     A factory for creating and managing basic single-color textures.
        /// </summary>
        public BasicTextureFactory BasicTextureFactory { get; private set; }

        /// <summary>
        ///     Gets the size of the window.
        /// </summary>
        private Point WindowSize
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
        }

        public override void Initialize()
        {
            BasicTextureFactory = new BasicTextureFactory(Game.GraphicsDevice);
            UIContentManager.CreateInstance(Game.Services);

            WindowSize = Game.Window.ClientBounds.Size;

            Texture2D whiteSmokeTexture = BasicTextureFactory.CreateBasicTexture(Color.WhiteSmoke);
            SpriteFont messageFont = Game.Content.Load<SpriteFont>("DebugFont");

            test = new Scrollbar()
            {
                Position = new Point(800, 200),
                LayerDepth = 1F,
                ScrollPosition = 50F,
                ScreenArea = new Rectangle(800, 200, 15, 500),
                Size = new Point(15, 500)
            };

            //Texture2D barBack = new Texture2D(Game.GraphicsDevice, 1, 1);
            //barBack.SetData(new Color[] { Color.WhiteSmoke });
            //Texture2D redRect = new Texture2D(Game.GraphicsDevice, 1, 1);
            //redRect.SetData(new Color[] { Color.Blue });

            //bar = new ProgressBar()
            //{
            //    AnchorPosition = AnchorPosition.BottomLeft,
            //    BackgroundTexture = barBack,
            //    BarTexture = redRect,
            //    Message = "Simulating",
            //    MessageFont = messageFont,
            //    RelativeLocation = new Point(20, -70),
            //    Size = new Point(WindowSize.X - 40, 50)
            //};

            //RootElement.AddChild(bar);

            //TextBox box = new TextBox()
            //{
            //    AnchorPosition = AnchorPosition.TopRight,
            //    BackgroundTexture = barBack,
            //    TextFont = messageFont,
            //    GraphicsDevice = GraphicsDevice,
            //    RelativeLocation = new Point(-420, 20),
            //    Size = new Point(400, 100)
            //};

            //Texture2D red = new Texture2D(GraphicsDevice, 1, 1);
            //red.SetData(new Color[] { Color.Red });
            //Texture2D green = new Texture2D(GraphicsDevice, 1, 1);
            //green.SetData(new Color[] { Color.Green });

            //box.Scrollbar.BackgroundTexture = redRect;
            //box.Scrollbar.ButtonTexture = green;
            //box.Scrollbar.HandleTexture = red;

            //box.AppendLine("Test line meme test");
            //box.AppendLine("Test line meme test meme test line meme line test.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("This is a test line, do not pay attention to the line behind the curtain.");
            //box.AppendLine("hottentottententententoonstellingsverzekeringsmaatschappij.");

            //RootElement.AddChild(box);

            //Button button = new Button()
            //{
            //    AnchorPosition = AnchorPosition.Center,
            //    BackgroundTexture = barBack,
            //    Text = "Click Me!",
            //    TextFont = messageFont,
            //    RelativeLocation = new Point(-50, -25),
            //    Size = new Point(100, 50),
            //    OnClick = () =>
            //    {
            //        if (box.Visible)
            //        {
            //            box.Visible = false;
            //        }
            //        else
            //        {
            //            box.Visible = true;
            //        }
            //    }
            //};

            //RootElement.AddChild(button);

            //InputTextBox kekBox = new InputTextBox()
            //{
            //    AnchorPosition = AnchorPosition.TopRight,
            //    BackgroundTexture = barBack,
            //    TextFont = messageFont,
            //    GraphicsDevice = GraphicsDevice,
            //    RelativeLocation = new Point(-420, 20),
            //    Size = new Point(400, 100)
            //};

            //kekBox.Scrollbar.BackgroundTexture = redRect;
            //kekBox.Scrollbar.ButtonTexture = green;
            //kekBox.Scrollbar.HandleTexture = red;

            //Game.Window.TextInput += kekBox.Window_TextInput;

            //RootElement.AddChild(kekBox);

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
            test.Update();
            base.Update(gameTime);
        }

        /// <summary>
        ///     Draw the UI on screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            test.Render(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
