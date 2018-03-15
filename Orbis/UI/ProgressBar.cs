using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Orbis.UI.BasicElements;

namespace Orbis.UI
{
    /// <summary>
    ///     A progress bar in the Orbis UI.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public class ProgressBar : UIElement
    {
        /// <summary>
        ///     Create a new <see cref="ProgressBar"/>.
        /// </summary>
        /// <param name="game">The game within which the progress bar exists.</param>
        public ProgressBar(Game game) : base(game)
        {
            Texture2D testTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            testTexture.SetData(new Color[] { Color.Red });

            var button = new Button(testTexture)
            {
                Position = new Point(200, 400),
                Size = new Point(80, 40),
                LayerDepth = 0.999F
            };

            _childElements.Add(button);
            _updatables.Add(button);
        }
        ///// <summary>
        /////     Get the progress bar's children (it should have none).
        ///// </summary>
        //public override UIElement[] Children
        //{
        //    get
        //    {
        //        return new UIElement[0];
        //    }
        //}

        ///// <summary>
        /////     The background texture of the progress bar.
        ///// </summary>
        //public Texture2D BackgroundTexture { get; set; }

        ///// <summary>
        /////     The texture of the actual progress bar.
        ///// </summary>
        //public Texture2D BarTexture { get; set; }

        ///// <summary>
        /////     The font used for drawing progress text.
        ///// </summary>
        //public SpriteFont MessageFont { get; set; }

        ///// <summary>
        /////     The message displayed above the progress bar.
        ///// </summary>
        //public string Message { get; set; }

        ///// <summary>
        /////     The color to use for drawing the message above the progress bar.
        ///// </summary>
        //public Color MessageColor { get; set; }

        ///// <summary>
        /////     Get the progress bar's current progress.
        ///// </summary>
        //public float Progress
        //{
        //    get
        //    {
        //        return _progress;
        //    }
        //    set
        //    {
        //        _progress = MathHelper.Clamp(value, 0, 100);
        //    }
        //}
        //private float _progress;

        ///// <summary>
        /////     The position and location of the actual loading bar.
        ///// </summary>
        //private Rectangle BarRectangle
        //{
        //    get
        //    {
        //        // Ensure absolute rectangle is only calculated once for this calculation.
        //        Rectangle absoluteRectangle = AbsoluteRectangle;

        //        Rectangle barRectangle = Rectangle.Empty;
        //        barRectangle.X = absoluteRectangle.X + 5;
        //        barRectangle.Y = absoluteRectangle.Y + 25;
        //        barRectangle.Height = absoluteRectangle.Height - 30;
        //        barRectangle.Width = (int)((absoluteRectangle.Width - 10) * Progress);

        //        return barRectangle;
        //    }
        //}

        ///// <summary>
        /////     Create a new <see cref="ProgressBar"/>.
        ///// </summary>
        //public ProgressBar() : base()
        //{
        //    Message = "Loading";
        //    MessageColor = Color.Black;
        //    _progress = 0.00F;
        //}

        ///// <summary>
        /////     The update method for the progress bar.
        ///// </summary>
        //public override void Update(GameTime gameTime)
        //{
        //    // No base update required; the item has no children.
        //}

        ///// <summary>
        /////     Draw the progress bar on the screen.
        ///// </summary>
        ///// <param name="spriteBatch"></param>
        ///// <param name="gameTime"></param>
        //public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    // Never draw if the progress bar is invisible.
        //    if (Visible)
        //    {
        //        // No drawing should be done if the required resources don't exist.
        //        if (spriteBatch != null && BarTexture != null && MessageFont != null)
        //        {
        //            if (BackgroundTexture != null)
        //            {
        //                spriteBatch.Draw(BackgroundTexture,
        //                    AbsoluteRectangle,
        //                    null,
        //                    Color.White,
        //                    0.00F,
        //                    Vector2.Zero,
        //                    SpriteEffects.None,
        //                    LayerDepth);
        //            }

        //            spriteBatch.Draw(BarTexture,
        //                BarRectangle,
        //                null,
        //                Color.White,
        //                0,
        //                Vector2.Zero,
        //                SpriteEffects.None,
        //                LayerDepth - 0.001F);

        //            // Make sure the absolute rectangle is only calculated once for this calculation.
        //            Rectangle absoluteRect = AbsoluteRectangle;
        //            Vector2 messagePosition = new Vector2(AbsoluteRectangle.Left + 5, AbsoluteRectangle.Top + 5);

        //            spriteBatch.DrawString(MessageFont,
        //                Message + " " + _progress.ToString("0.00%"),
        //                messagePosition,
        //                MessageColor,
        //                0.00F,
        //                Vector2.Zero,
        //                1.00F,
        //                SpriteEffects.None,
        //                LayerDepth - 0.001F);
        //        }
        //    }
        //    // No base draw is required; the progress bar has no children.
        //}
    }
}
