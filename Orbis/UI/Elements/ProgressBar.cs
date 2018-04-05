using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Orbis.UI.Utility;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     A progress bar in the Orbis UI.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public class ProgressBar : RelativeElement, IRenderableElement
    {
        // Used to draw the progress message on the screen.
        private RelativeText _progressMessage;

        // Used to draw the progress bar on the screen.
        private RelativeTexture _progressBar;

        /// <summary>
        ///     The message displayed above the progress bar.
        /// </summary>
        public string Message { get; set; }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }
        private bool visible = true;

        /// <summary>
        ///     The color to use for drawing the message above the progress bar.
        /// </summary>
        public Color MessageColor { get => _progressMessage.TextColor; set => _progressMessage.TextColor = value; }

        /// <summary>
        ///     Get the progress bar's current progress.
        /// </summary>
        public float Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                // Value is limited between 0 and 100 because it is a percentage from 0 to 100%.
                _progress = MathHelper.Clamp(value, 0, 100);

                // Bar size needs to be recalculated.
                Point barSize = Point.Zero;
                // Pixel values are floored to get consistent results.
                barSize.X = (int)Math.Floor((Bounds.Width - 8) * _progress);
                barSize.Y = Bounds.Height - 30;

                _progressBar.Size = barSize;

                _progressMessage.Text = Message;
            }
        }
        private float _progress;

        /// <summary>
        ///     The position and location of the actual loading bar.
        /// </summary>
        private Rectangle BarRectangle
        {
            get
            {
                // Ensure absolute rectangle is only calculated once for this calculation.
                Rectangle absoluteRectangle = Bounds;

                Rectangle barRectangle = Rectangle.Empty;
                barRectangle.X = absoluteRectangle.X + 5;
                barRectangle.Y = absoluteRectangle.Y + 25;
                barRectangle.Height = absoluteRectangle.Height - 30;
                barRectangle.Width = (int)((absoluteRectangle.Width - 10) * Progress);

                return barRectangle;
            }
        }

        /// <summary>
        ///     The layer depth of the progress bar.
        /// </summary>
        /// 
        /// <remarks>
        ///     Value should be between 0 and 1, with zero being the front and 1 being the back.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException" />
        public float LayerDepth
        {
            get
            {
                return _progressBar.LayerDepth;
            }
            set
            {
                _progressBar.LayerDepth = value;
                _progressMessage.LayerDepth = value;
            }
        }

        /// <summary>
        ///     Create a new <see cref="ProgressBar"/>.
        /// </summary>
        public ProgressBar(IPositionedElement parent) : base(parent)
        {
            if (UIContentManager.TryGetInstance(out UIContentManager manager))
            {
                SpriteFont messageFont = manager.GetFont("DebugFont");
                Texture2D redTexture = manager.GetColorTexture(Color.Red);
                SpriteDefinition barDef = new SpriteDefinition(redTexture, new Rectangle(0, 0, 1, 1));

                _progressMessage = new RelativeText(this, messageFont)
                {
                    RelativePosition = new Point(5, 5)
                };
                _progressBar = new RelativeTexture(this, barDef)
                {
                    RelativePosition = new Point(5, 30)
                };
                Message = "Loading";
                MessageColor = Color.Black;
                Progress = 0.00F;
            }
            else
            {
                throw new InvalidOperationException("GUI Content manager could not be retrieved.");
            }
        }

        /// <summary>
        ///     Render the progress bar.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Render(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                _progressMessage.Render(spriteBatch);
                _progressBar.Render(spriteBatch);
            }
        }
    }
}
