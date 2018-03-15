using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.BasicElements;
using System;
using System.Collections.Generic;

namespace Orbis.UI
{
    /// <summary>
    ///     Represents an element in the Orbis UI.
    /// </summary>
    /// <author>Kaj van der Veen</author>
    public abstract class UIElement
    {
        // Used to save the basic elements making up this element.
        protected List<IBasicElement> _childElements;

        // Used to draw the element.
        protected Texture2D _elementTexture;

        // Used to indicate that element needs to be rerendered the next frame.
        private bool _isInvalidated;

        // Used to keep track of children that can be updated.
        protected List<IUpdatableElement> _updatables;

        /// <summary>
        ///     The anchor mode for this element. Decides where the element anchors to the window.
        /// </summary>
        public AnchorPosition AnchorPosition { get; set; }

        /// <summary>
        ///     The relative position of the UI element.
        /// </summary>
        public Point RelativePosition { get; set; }

        /// <summary>
        ///     The on-screen position of the element.
        /// </summary>
        public Point ScreenPosition
        {
            get
            {
                Point location = new Point(RelativePosition.X, RelativePosition.Y);
                Rectangle windowSize = Window.ClientBounds;

                switch (AnchorPosition)
                {
                    case AnchorPosition.TopRight:
                        location.X += windowSize.Right;
                        break;
                    case AnchorPosition.Center:
                        location += windowSize.Center;
                        break;
                    case AnchorPosition.BottomLeft:
                        location.Y += windowSize.Bottom;
                        break;
                    case AnchorPosition.BottomRight:
                        location.X += windowSize.Right;
                        location.Y += windowSize.Bottom;
                        break;

                    case AnchorPosition.TopLeft:
                    default:
                        // The top left requires no changes.
                        break;
                }

                return location;
                //// Anchor positions decide what point of the parent the element is relative to.
                //if (AnchorPosition == AnchorPosition.TopLeft)
                //{
                //    location.Y;
                //}
                //else if (AnchorPosition == AnchorPosition.TopRight)
                //{
                //    Point parentTopRight = new Point(parentRect.Right, parentRect.Top);
                //    absoluteRect.Location = absoluteRect.Location + parentTopRight;
                //}
                //else if (AnchorPosition == AnchorPosition.Center)
                //{
                //    absoluteRect.Location = absoluteRect.Location + parentRect.Center;
                //}
                //else if (AnchorPosition == AnchorPosition.BottomLeft)
                //{
                //    absoluteRect.Y += parentRect.Bottom;
                //}
                //else if (AnchorPosition == AnchorPosition.BottomRight)
                //{
                //    Point parentBottomRight = new Point(parentRect.Right, parentRect.Bottom);
                //    absoluteRect.Location = absoluteRect.Location + parentBottomRight;
                //}

                //return absoluteRect;
            }
        }

        /// <summary>
        ///     The size of the UI element.
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        ///     Is the UI Element visible?
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     The game window.
        /// </summary>
        public GameWindow Window { get; }

        /// <summary>
        ///     Create a new <see cref="UIElement"/>.
        /// </summary>
        public UIElement(Game game) {
            if (game == null)
            {
                throw new ArgumentNullException();
            }

            AnchorPosition = AnchorPosition.TopLeft;

            _childElements = new List<IBasicElement>();
            _updatables = new List<IUpdatableElement>();
            _isInvalidated = true;

            
            Window = game.Window;
            RelativePosition = Point.Zero;
            Size = Point.Zero;
            Visible = true;
        }

        /// <summary>
        ///     Rerender the UI Element.
        /// </summary>
        protected virtual void Rerender(SpriteBatch spriteBatch)
        {
            // Prepare the graphics device for drawing the element texture to a new render target.
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            var prevRenderTargets = graphicsDevice.GetRenderTargets();
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, Size.X, Size.Y);
            graphicsDevice.SetRenderTarget(renderTarget);

            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();
            spriteBatch.Begin();
            foreach (IBasicElement child in _childElements)
            {
                child.Render(spriteBatch);
            }
            spriteBatch.End();
            spriteBatch.Begin();
            graphicsDevice.SetRenderTargets(prevRenderTargets);

            _elementTexture = renderTarget;
        }

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public virtual void Update()
        {
            foreach (IUpdatableElement child in _updatables)
            {
                child.Update();
            }
        }

        /// <summary>
        ///     Draw the UI Element.
        /// </summary>
        /// 
        /// <param name="spriteBatch">
        ///     The SpriteBatch to use for drawing textures.
        /// </param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_isInvalidated)
            {
                Rerender(spriteBatch);
                _isInvalidated = false;
            }

            spriteBatch.Draw(
                _elementTexture,
                new Rectangle(ScreenPosition, Size),
                null,
                Color.White,
                0F,
                Vector2.Zero,
                SpriteEffects.None,
                0F);
        }
    }
}
