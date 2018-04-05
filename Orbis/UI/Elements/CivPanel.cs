using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.Simulation;
using Orbis.UI.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     A panel that displays the stats of the different civs in the simulation.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public class CivPanel : RelativeElement, IRenderableElement, IUpdatableElement
    {
        // Used to keep track of the entries in the panel.
        private Dictionary<Civilization, Entry> _civTexturePairs;

        // Used to handle overflow and scrolling.
        private RenderTarget2D _fullTexture;
        private Scrollbar _scrollbar;
        private Rectangle _viewPort;

        // Used for the text in the panel.
        private string _civText = "a";
        private SpriteFont _textFont;

        /// <summary>
        ///     The size of the civ panel.
        /// </summary>
        public override Point Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                _viewPort.Size = value;
                _scrollbar.Size = new Point(15, Size.Y);
            }
        }

        /// <summary>
        ///     The layer depth of the civ panel.
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
                return _scrollbar.LayerDepth;
            }
            set
            {
                _scrollbar.LayerDepth = value;
            }
        }

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
        ///     Create a new <see cref="CivPanel"/>.
        /// </summary>
        /// 
        /// <param name="parent">
        ///     The parent within which the panel will be displayed.
        /// </param>
        /// <param name="civs">
        ///     The civs in the simulation.
        /// </param>
        public CivPanel(IPositionedElement parent, IEnumerable<Civilization> civs) : base(parent)
        {
            _civTexturePairs = new Dictionary<Civilization, Entry>();
            _viewPort = Rectangle.Empty;

            if (UIContentManager.TryGetInstance(out UIContentManager manager))
            {
                _textFont = manager.GetFont("DebugFont");
            }

            _scrollbar = new Scrollbar(this)
            {
                AnchorPosition = AnchorPosition.TopRight,
                Size = new Point(15, this.Size.Y),
                RelativePosition = new Point(-15, 0)
            };

            foreach (Civilization civ in civs)
            {
                _civTexturePairs.Add(civ, new Entry() {
                    Texture = manager.GetColorTexture(civ.Color)
                });
            }
        }

        /// <summary>
        ///     Perform the pre-render procedure for the civPanel.
        /// </summary>
        public void PreRender(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            int fullTextHeight = (int)Math.Ceiling(_textFont.MeasureString(_civText).Y);

            // A new render target needs to be created if the dimensions have changed or if it doesn't exist.
            if (_fullTexture == null || _fullTexture.Height != fullTextHeight)
            {
                if (_fullTexture != null)
                {
                    // Clean up used resources if it exists.
                    _fullTexture.Dispose();
                }
                _fullTexture = new RenderTarget2D(spriteBatch.GraphicsDevice, Size.X - 25, fullTextHeight);
            }

            // The spritebatch will be used to draw the elements to a texture that will be used to keep overflow for scrolling.
            spriteBatch.GraphicsDevice.SetRenderTarget(_fullTexture);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.BackToFront);
            foreach (var civTexturePair in _civTexturePairs)
            {
                var civEntry = civTexturePair.Value;
                spriteBatch.Draw(civEntry.Texture,
                    new Rectangle(civEntry.EntryRect.Location, new Point(5, civEntry.EntryRect.Height)),
                    null,
                    Color.White,
                    0F,
                    Vector2.Zero,
                    SpriteEffects.None,
                    LayerDepth - 0.001F);
            }

            spriteBatch.DrawString(_textFont, _civText, new Vector2(15, 0), Color.Black);

            spriteBatch.End();

            // The graphics device is reset to draw to the back bufer.
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            // The spritebatch is started again to continue the previous flow of drawing on the back buffer.
            spriteBatch.Begin(SpriteSortMode.BackToFront);
        }

        /// <summary>
        ///     Render the civ panel.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spritebatch used for drawing.
        /// </param>
        public void Render(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                // To allow scrolling, the overflowing elements need to be rendered to a render target first.
                PreRender(spriteBatch);

                int textHeight = (int)Math.Ceiling(_textFont.MeasureString(_civText).Y);
                if (textHeight > Size.Y)
                {
                    _scrollbar.IsFocused = true;
                    _scrollbar.Render(spriteBatch);
                }
                else
                {
                    _scrollbar.IsFocused = false;
                }

                spriteBatch.Draw(_fullTexture,
                    Bounds,
                    _viewPort,
                    Color.White, 0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    LayerDepth - 0.01F);
            }
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public void Update()
        {
            _scrollbar.Update();

            // Every entry in the list needs to be calculated for this frame.
            var totalOffset = 0;
            StringBuilder fullCivText = new StringBuilder();
            foreach (var civTexturePair in _civTexturePairs)
            {
                var civ = civTexturePair.Key;
                StringBuilder entrySb = new StringBuilder();

                entrySb.AppendLine(TextHelper.WrapText(_textFont, civ.Name, Size.X - 30));
                entrySb.AppendLine("  Is Alive: " + civ.IsAlive);
                entrySb.AppendLine("  Population: " + civ.Population);
                entrySb.AppendLine("  Size: " + (civ.Territory.Count * 3141) + " KM^2");
                entrySb.AppendLine("  Wealth: " + (int)civ.TotalWealth + "KG AU");
                entrySb.Append("  Resources: " + (int)civ.TotalResource + " KG");

                string entryText = entrySb.ToString();

                // An entry is used to keep track of related values.
                var civEntry = civTexturePair.Value;
                Vector2 textSize = _textFont.MeasureString(entryText);
                civEntry.Text = entryText;
                civEntry.EntryRect = new Rectangle(5, totalOffset, Size.X - 25, (int)Math.Ceiling(textSize.Y));

                fullCivText.AppendLine(entryText);
                fullCivText.AppendLine();

                totalOffset += (int)Math.Ceiling(textSize.Y + 18);
            }

            _civText = fullCivText.ToString();

            int fullTextHeight = (int)Math.Ceiling(_textFont.MeasureString(_civText).Y) - 18;
            _viewPort.Y = (int)Math.Floor(0 + (_scrollbar.ScrollPosition / 100) * (fullTextHeight - Size.Y));
        }

        /// <summary>
        ///     Used to keep track of entries in the civ panel. 
        /// </summary>
        /// 
        /// <remarks>
        ///     Used to be a struct, but the garbage collector kept deleting the values.
        /// </remarks>
        private class Entry
        {
            /// <summary>
            ///     The text displayed in this entry.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            ///     The texture of the colored bar in the entry.
            /// </summary>
            public Texture2D Texture { get; set; }

            /// <summary>
            ///     The position and size of the entry.
            /// </summary>
            public Rectangle EntryRect { get; set; }
        }
    }
}
