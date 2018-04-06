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
    public class CivPanel : RelativeElement, IRenderableElement, IUpdateableElement
    {
        // Used to keep track of the entries in the panel.
        private Dictionary<Civilization, Entry> _civTexturePairs;

        // Used to clip the overflow of the panel.
        private RasterizerState _clipState;

        // Used to handle overflow and scrolling.
        private Scrollbar _scrollbar;
        private int _scrollOffset;

        // Used for the text in the panel.
        private SpriteFont _textFont;
        private RelativeText _civText;

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

        /// <summary>
        ///     Is the civ panel visible?
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Is the civ panel in focus?
        /// </summary>
        public bool Focused { get; set; }

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
            if (UIContentManager.TryGetInstance(out UIContentManager manager))
            {
                _civTexturePairs = new Dictionary<Civilization, Entry>();
                _scrollOffset = 0;
                Visible = true;
                Focused = true;
                _textFont = manager.GetFont("DebugFont");
                _civText = new RelativeText(this, _textFont);

                _scrollbar = new Scrollbar(this)
                {
                    AnchorPosition = AnchorPosition.TopRight,
                    Size = new Point(15, this.Size.Y),
                    RelativePosition = new Point(-15, 0)
                };

                foreach (Civilization civ in civs)
                {
                    _civTexturePairs.Add(civ, new Entry()
                    {
                        Texture = new RelativeTexture(this, new SpriteDefinition(manager.GetColorTexture(civ.Color), new Rectangle(0, 0, 1, 1)))
                    });
                }
            }
            else
            {
                throw new InvalidOperationException("UI Content manager does not exist.");
            }
            
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
                int textHeight = _civText.Size.Y;
                if (textHeight > Size.Y)
                {
                    _scrollbar.Focused = true;
                    _scrollbar.Render(spriteBatch);
                }
                else
                {
                    _scrollbar.Focused = false;
                }

                spriteBatch.End();

                if (_clipState == null)
                {
                    _clipState = new RasterizerState()
                    {
                        ScissorTestEnable = true
                    };
                }

                RasterizerState prevRasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                spriteBatch.GraphicsDevice.ScissorRectangle = Bounds;

                spriteBatch.Begin(SpriteSortMode.BackToFront, rasterizerState: _clipState);
                foreach (var civTexturePair in _civTexturePairs)
                {

                    var civEntry = civTexturePair.Value;
                    civEntry.Texture.Render(spriteBatch);
                }

                _civText.Render(spriteBatch);

                spriteBatch.End();

                // The spritebatch is started again to continue the previous flow of drawing.
                spriteBatch.Begin(SpriteSortMode.BackToFront, rasterizerState: prevRasterizerState);
            }
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public void Update()
        {
            _scrollbar.Update();

            int fullTextHeight = _civText.Size.Y;
            _scrollbar.ScrollLength = fullTextHeight;
            _scrollOffset = (int)Math.Floor(0 + ((_scrollbar.ScrollPosition / 100)) * (fullTextHeight - Size.Y));

            // Every entry in the list needs to be calculated for this frame.
            var totalOffset = 0;
            StringBuilder fullCivText = new StringBuilder();
            foreach (var civTexturePair in _civTexturePairs)
            {
                var civ = civTexturePair.Key;
                StringBuilder entrySb = new StringBuilder();

                entrySb.AppendLine(TextHelper.WrapText(_textFont, civ.Name, Size.X - 30));
                entrySb.AppendLine("  Is Alive: " + civ.IsAlive);
                entrySb.AppendLine("  Is at war: " + civ.AtWar);
                entrySb.AppendLine("  Population: " + civ.Population);
                entrySb.AppendLine("  Size: " + (civ.Territory.Count * 3141) + " KM^2");
                entrySb.AppendLine("  Wealth: " + (int)civ.TotalWealth + " KG AU");
                entrySb.Append("  Resources: " + (int)civ.TotalResource + " KG");

                string entryText = entrySb.ToString();

                // An entry is used to keep track of related values.
                var civEntry = civTexturePair.Value;
                Vector2 textSize = _textFont.MeasureString(entryText);
                civEntry.Text = entryText;

                civEntry.Texture.Size = new Point(5, (int)Math.Ceiling(textSize.Y));
                civEntry.Texture.RelativePosition = new Point(5, totalOffset - _scrollOffset);

                fullCivText.AppendLine(entryText);
                fullCivText.AppendLine();

                totalOffset += (int)Math.Ceiling(textSize.Y + _textFont.LineSpacing);
            }

            _civText.Text = fullCivText.ToString();
            _civText.RelativePosition = new Point(15, 0 - _scrollOffset);
        }

        /// <summary>
        ///     Used to keep track of entries in the civ panel. 
        /// </summary>
        /// 
        /// <remarks>
        ///     Used to be a struct, but the garbage collector kept deleting the values.
        /// </remarks>
        private struct Entry
        {
            /// <summary>
            ///     The text displayed in this entry.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            ///     The texture of the colored bar in the entry.
            /// </summary>
            public RelativeTexture Texture { get; set; }
        }
    }
}
