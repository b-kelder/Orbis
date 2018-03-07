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
    public class UIWindow : DrawableGameComponent
    {
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
        public UIWindow(Game game) : base(game)
        {
            RootElement = new SplitPanel()
            {
                FixedChild = 1,
                SplitDirection = SplitDirection.Vertical,
                Split = 200
            };
        }

        public override void Initialize()
        {
            WindowSize = Game.Window.ClientBounds.Size;
            RootElement.Size = WindowSize;

            var rightChild = new SplitPanel()
            {
                Split = 200,
                SplitDirection = SplitDirection.Horizontal,
                FixedChild = 0
            };
            RootElement.AddChild(rightChild);

            var redRect = new Texture2D(GraphicsDevice, 1, 1);
            redRect.SetData(new[] { Color.Red });
            rightChild.AddChild(new Panel()
            {
                BackgroundTexture = redRect
            });

            var yellowRect = new Texture2D(GraphicsDevice, 1, 1);
            yellowRect.SetData(new[] { Color.Yellow });
            rightChild.AddChild(new Panel()
            {
                BackgroundTexture = yellowRect
            });

            var blueRect = new Texture2D(GraphicsDevice, 1, 1);
            blueRect.SetData(new[] { Color.Blue });
            RootElement.AddChild(new Panel()
            {
                BackgroundTexture = blueRect
            });

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var newWindowSize = Game.Window.ClientBounds.Size;
            if (WindowSize != newWindowSize)
            {
                RootElement.Size = newWindowSize;
                WindowSize = newWindowSize;
            }

            System.Diagnostics.Debug.WriteLine(WindowSize);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            RootElement.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
