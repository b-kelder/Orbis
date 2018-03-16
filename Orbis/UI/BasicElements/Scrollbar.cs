using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Orbis.Engine;
using System;

namespace Orbis.UI.BasicElements
{
    /// <summary>
    ///     Represents a scrollbar that can be used by UI Elements.
    /// </summary>
    public class Scrollbar : IRenderableElement, IUpdatableElement
    {
        // Used to display the background.
        private PositionedTexture _background;

        // Used for moving the scrollbar down.
        private Button _downButton;

        // Used to display the handle;
        private PositionedTexture _handle;

        // Used for moving the scrollbar up.
        private Button _upButton;

        /// <summary>
        ///     The bounds of the scrollbar.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position, Size);
            }
        }

        /// <summary>
        ///     The layer depth of the scrollbar.
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
                return _background.LayerDepth;
            }
            set
            {
                _background.LayerDepth = value;
                _upButton.LayerDepth = value;
                _downButton.LayerDepth = value;

                // The handle is drawn slightly above the background.
                _handle.LayerDepth = value - 0.001F;
            }
        }

        /// <summary>
        ///     The position of the scrollbar.
        /// </summary>
        public Point Position
        {
            get
            {
                return _background.Position + new Point(0, -15);
            }
            set
            {
                _background.Position = value + new Point(0, 15);
                _upButton.Position = value;
                _downButton.Position = value + new Point(0, Size.Y - 15);

                // Handle is positioned at the rounded relative value of the scroll position.
                _handle.Position = value + new Point(0, (int)Math.Floor((Size.Y - 30) * (ScrollPosition / 100)));
            }
        }

        /// <summary>
        ///     The relative value of the handle's position within the scrollbar.
        /// </summary>
        /// 
        /// <remarks>
        ///     The scroll position is limited to the range 0 - 100.
        /// </remarks>
        public float ScrollPosition
        {
            get
            {
                return _scrollPos;
            }
            set
            {
                // Restrict value between 0 and 100.
                _scrollPos = MathHelper.Clamp(value, 0F, 100F);
            }
        }
        private float _scrollPos;

        /// <summary>
        ///     The size of the scrollbar.
        /// </summary>
        public Point Size
        {
            get
            {
                return _background.Size + new Point(0, 30);
            }
            set
            {
                _background.Size = (value.X == 15) ? value + new Point(0, -30)
                    : throw new ArgumentOutOfRangeException("Scrollbar can not be wider than 15px.");
            }
        }

        /// <summary>
        ///     Create a new <see cref="Scrollbar"/>.
        /// </summary>
        /// 
        /// <exception cref="Exception" />
        public Scrollbar()
        {   
            // Before the scrollbar items can be created, the required resources need to be created.
            Texture2D scrollbarSheet;
            SpriteFont defaultFont;
            if (UIContentManager.TryGetInstance(out UIContentManager manager))
            {
                scrollbarSheet = manager.GetTexture("UI\\Scrollbar");
                defaultFont = manager.GetFont("DebugFont");
            }
            else
            {
                throw new Exception("Could not retrieve scrollbar spritesheet.");
            }

            // Sprite definitions are created for the different sprites used by scrollbar items.
            Rectangle buttonRect = new Rectangle(0, 0, 15, 15);
            SpriteDefinition buttonDef = new SpriteDefinition(scrollbarSheet, buttonRect);

            Rectangle handleRect = new Rectangle(0, 15, 15, 20);
            SpriteDefinition handleDef = new SpriteDefinition(scrollbarSheet, handleRect);

            Rectangle backgroundRect = new Rectangle(15, 0, 15, 35);
            SpriteDefinition backgroundDef = new SpriteDefinition(scrollbarSheet, backgroundRect);

            // Finally, the items themselves can be created.
            _upButton = new Button(buttonDef, defaultFont);
            _downButton = new Button(buttonDef, defaultFont)
            {
                // Down button uses same texture but flipped.
                SpriteEffects = SpriteEffects.FlipVertically
            };
            _handle = new PositionedTexture(handleDef);
            _background = new PositionedTexture(backgroundDef);

            _downButton.Hold += _downButton_Hold;
            _upButton.Hold += _upButton_Hold;

            ScrollPosition = 0F;
        }

        /// <summary>
        ///     Event handler for holding the mouse down over the down button.
        /// </summary>
        private void _downButton_Hold(object sender, EventArgs e)
        {
            ScrollPosition = MathHelper.Clamp(ScrollPosition + 1F, 0F, 100F);

            // Handle is positioned at the rounded relative value of the scroll position.
            _handle.Position = Position + new Point(0, (int)Math.Floor((Size.Y - 30) * (ScrollPosition / 100)));
        }

        /// <summary>
        ///     Event handler for holding the mouse down over the up button.
        /// </summary>
        private void _upButton_Hold(object sender, EventArgs e)
        {
            ScrollPosition = MathHelper.Clamp(ScrollPosition - 1F, 0F, 100F);

            // Handle is positioned at the rounded relative value of the scroll position.
            _handle.Position = Position + new Point(0, (int)Math.Floor((Size.Y - 30) * (ScrollPosition / 100)));
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public void Update()
        {
            _upButton.Update();
            _downButton.Update();
        }

        /// <summary>
        ///     Render the scrollbar using the given spriteBatch.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The spriteBatch to use for drawing;
        /// </param>
        public void Render(SpriteBatch spriteBatch)
        {
            _upButton.Render(spriteBatch);
            _background.Render(spriteBatch);
            _downButton.Render(spriteBatch);

            // Handle is positioned at the rounded relative value of the scroll position.
            _handle.Position = Position + new Point(0, (int)Math.Floor((Size.Y - 30) * (ScrollPosition / 100)));
            _handle.Render(spriteBatch);
        }
        //    /// <summary>
        //    ///     The texture used as the background for the scrollbar.
        //    /// </summary>
        //    public Texture2D BackgroundTexture { get; set; }

        //    /// <summary>
        //    ///     The texture used for the handle of the scrollbar.
        //    /// </summary>
        //    public Texture2D HandleTexture { get; set; }

        //    /// <summary>
        //    ///     The texture used for the buttons of the scrollbar.
        //    /// </summary>
        //    /// 
        //    /// <remarks>
        //    ///     The proper texture for a scroll button is a 15x15 upward-facing arrow texture.
        //    /// </remarks>
        //    public Texture2D ButtonTexture { get; set; }

        //    /// <summary>
        //    ///     The position of the scrollbar's handle.
        //    /// </summary>
        //    public float HandlePosition { get; private set; }

        //    /// <summary>
        //    ///     Is the scrollbar horizontal?
        //    /// </summary>
        //    public bool IsHorizontal { get; set; }

        //    // The absolute position and size of the buttons used to navigate the scrollbar.
        //    private Rectangle _upButton;
        //    private Rectangle _handle;
        //    private Rectangle _downButton;

        //    /// <summary>
        //    ///     Create a new <see cref="Scrollbar"/>.
        //    /// </summary>
        //    public Scrollbar() : base()
        //    {
        //        HandlePosition = 0F;
        //    }

        //    /// <summary>
        //    ///     Perform update actions for the scrollbar.
        //    /// </summary>
        //    /// 
        //    /// <param name="gameTime">
        //    ///     The game loop's current game time.
        //    /// </param>
        //    public void Update(GameTime gameTime)
        //    {
        //        // Scrollbar handles input through the buttons at the top and bottom.
        //        InputHandler input = InputHandler.GetInstance();
        //        Point mousePosition = Mouse.GetState().Position;
        //        if (_upButton.Contains(mousePosition))
        //        {
        //            if (input.IsMouseHold(MouseButton.Left))
        //            {
        //                HandlePosition = MathHelper.Clamp(HandlePosition - 1F, 0, 100);
        //                UpdateLayout();
        //            }
        //        }
        //        else if (_downButton.Contains(mousePosition))
        //        {
        //            if (input.IsMouseHold(MouseButton.Left))
        //            {
        //                HandlePosition = MathHelper.Clamp(HandlePosition + 1F, 0, 100);
        //                UpdateLayout();
        //            }
        //        }
        //        // No base Update needed; scrollbars do not have children.
        //    }

        //    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //    {
        //        // Draw only if the scrollbar is visible.
        //        if (Visible)
        //        {
        //            // Drawing will not happen if the required resources are not assigned.
        //            if (spriteBatch != null && BackgroundTexture != null && HandleTexture != null && ButtonTexture != null)
        //            {
        //                spriteBatch.Draw(BackgroundTexture,
        //                    AbsoluteRectangle,
        //                    null,
        //                    Color.White,
        //                    0F,
        //                    Vector2.Zero,
        //                    SpriteEffects.None,
        //                    LayerDepth);

        //                spriteBatch.Draw(ButtonTexture,
        //                    _upButton,
        //                    null,
        //                    Color.White,
        //                    0F,
        //                    Vector2.Zero,
        //                    SpriteEffects.None,
        //                    LayerDepth - 0.001F);

        //                spriteBatch.Draw(HandleTexture,
        //                    _handle,
        //                    null,
        //                    Color.White,
        //                    0F,
        //                    Vector2.Zero,
        //                    SpriteEffects.None,
        //                    LayerDepth - 0.001F);

        //                spriteBatch.Draw(ButtonTexture,
        //                    _downButton,
        //                    null,
        //                    Color.White,
        //                    0F,
        //                    Vector2.Zero,
        //                    SpriteEffects.FlipVertically,
        //                    LayerDepth - 0.001F);
        //            }
        //        }
        //    }

        //    /// <summary>
        //    ///     Scrollbars can not have children; do not use.
        //    /// </summary>
        //    /// <exception cref="OrbisUIException" />
        //    public override void AddChild(UIElement child)
        //    {
        //        throw new OrbisUIException("Scrollbars can not have children.");
        //    }

        //    /// <summary>
        //    ///     Scrollbars can not have children; do not use.
        //    /// </summary>
        //    /// <exception cref="OrbisUIException" />
        //    public override void ReplaceChild(int childIndex, UIElement newChild)
        //    {
        //        throw new OrbisUIException("Scrollbars can not have children.");
        //    }

        //    /// <summary>
        //    ///     Update the layout of the scroll bar.
        //    /// </summary>
        //    public override void UpdateLayout()
        //    {
        //        // Don't bother updating these things if the scrollbar isn't visible.
        //        if (Visible)
        //        {
        //            // Ensure the absolute position is only calculated once.
        //            Rectangle absoluteRectangle = AbsoluteRectangle;

        //            // Set the positions of the buttons and handle based on the absolute position of the scrollbar.
        //            _upButton = new Rectangle(absoluteRectangle.X, absoluteRectangle.Y, 15, 15);
        //            int handleY = (int)Math.Floor(absoluteRectangle.Y + ((Size.Y - 50) * (HandlePosition / 100)));
        //            _handle = new Rectangle(absoluteRectangle.X, handleY + 15, 15, 20);
        //            _downButton = new Rectangle(absoluteRectangle.X, absoluteRectangle.Bottom - 15, 15, 15);
        //        }

        //        // No base UpdateLayout needed; scrollbars do not have children;
        //    }
    }
}
