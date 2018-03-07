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
            
        }

        public override void Initialize()
        {
            WindowSize = Game.Window.ClientBounds.Size;
            RootElement.Size = WindowSize;

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var newWindowSize = Game.Window.ClientBounds.Size;
            if (WindowSize != newWindowSize)
            {
                RootElement.Size = newWindowSize;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            RootElement.Draw(gameTime);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
