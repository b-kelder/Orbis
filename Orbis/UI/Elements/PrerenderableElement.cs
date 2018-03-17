using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Elements;
using System;
using System.Collections.Generic;

namespace Orbis.UI.Elements
{
    /// <summary>
    ///     Abstract implementation for a complex element that can be pre-rendered.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public abstract class PrerenderableElement : RelativeElement, IUpdatableElement
    {
        // Used to save the basic elements making up this element.
        //protected List<IUIElement> _childElements;

        // Used to draw the element.
        protected Texture2D _elementTexture;

        // Used to indicate that element needs to be rerendered the next frame.
        protected bool _isInvalidated;

        // Used to keep track of children that can be updated.
        protected List<IUpdatableElement> _updatables;

        /// <summary>
        ///     Is the UI Element visible?
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        ///     Create a new <see cref="PrerenderableElement"/>.
        /// </summary>
        public PrerenderableElement(IPositionedElement parent, Game game) : base(parent)
        {
            if (game == null)
            {
                throw new ArgumentNullException();
            }

            AnchorPosition = AnchorPosition.TopLeft;

            //_childElements = new List<IUIElement>();
            _updatables = new List<IUpdatableElement>();
            _isInvalidated = true;

            RelativePosition = Point.Zero;
            Size = Point.Zero;
            Visible = true;
        }

        /// <summary>
        ///     Prerender the static elements in the 
        /// </summary>
        public virtual void Prerender(SpriteBatch spriteBatch)
        {
            if (_isInvalidated)
            {
                // Prepare the graphics device for drawing the element texture to a new render target.
                GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
                var prevRenderTargets = graphicsDevice.GetRenderTargets();
                RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, Size.X, Size.Y);
                graphicsDevice.SetRenderTarget(renderTarget);

                graphicsDevice.Clear(Color.Transparent);
                //foreach (IUIElement child in _childElements)
                //{
                //    child.Render(spriteBatch);
                //}
                graphicsDevice.SetRenderTargets(prevRenderTargets);

                _elementTexture = renderTarget;
            }
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
            spriteBatch.Draw(
                _elementTexture,
                Bounds,
                null,
                Color.White,
                0F,
                Vector2.Zero,
                SpriteEffects.None,
                0F);
        }
    }
}
