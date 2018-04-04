using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Orbis.UI.Elements;

namespace Orbis.UI.Windows
{
    public class TestWindow : UIWindow
    {
        public ProgressBar TestBar { get; set; }

        public TestWindow(Game game) : base(game)
        {
            TestBar = new ProgressBar(this)
            {
                AnchorPosition = AnchorPosition.BottomLeft,
                RelativePosition = new Point(20, -70),
                Size = new Point(_game.Window.ClientBounds.Width - 40, 50)
            };
        }

        /// <summary>
        ///     Draw the TestWindow.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            TestBar.Render(spriteBatch);
        }

        /// <summary>
        ///     Event handler for window resizing.
        /// </summary>
        protected override void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            TestBar.Size = new Point(_game.Window.ClientBounds.Width - 40, 50);
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public override void Update()
        {
            // This particular window has no update actions to perform.
        }
    }
}
