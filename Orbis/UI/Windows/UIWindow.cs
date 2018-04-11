using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orbis.UI.Utility;
using System;
using System.Collections.Generic;

namespace Orbis.UI.Windows
{
    /// <summary>
    ///     Abstract implementation of a window in the Orbis UI.
    /// </summary>
    /// 
    /// <author>Kaj van der Veen</author>
    public abstract class UIWindow : IPositionedElement, IUpdateableElement
    {
        // Used for getting window size and text input.
        protected Game _game;

        protected List<IRenderableElement> _children;
        protected List<IUpdateableElement> _updateableChildren;

        protected UIContentManager _contentManager;

        /// <summary>
        ///     The screen bounds of the window.
        /// </summary>
        public Rectangle Bounds => _game.Window.ClientBounds; // Windows have the same bounds as the game window.

        /// <summary>
        ///     The size of the window.
        /// </summary>
        public Point Size => _game.Window.ClientBounds.Size; // Windows are the same size as the game window.

        /// <summary>
        ///     The position of the window.
        /// </summary>
        public Point Position => Point.Zero; // Windows always have the position (0,0).

        /// <summary>
        ///     Is the window in focus? (yes)
        /// </summary>
        public bool Focused
        {
            get
            {
                return true;
            }
            set { } // Windows are always focused so they can't be set.
        }

        /// <summary>
        ///     Create a new <see cref="UIWindow"/>.
        /// </summary>
        /// <param name="game"></param>
        public UIWindow(Game game)
        {
            if (!UIContentManager.TryGetInstance(out _contentManager))
            {
                throw new InvalidOperationException("Window could not retrieve UI content manager.");
            }

            _game = game;
            _game.Window.ClientSizeChanged += Window_ClientSizeChanged;
            _children = new List<IRenderableElement>();
            _updateableChildren = new List<IUpdateableElement>();
        }

        /// <summary>
        ///     Draw the UIWindow to the screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use for drawing.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var child in _children)
            {
                child.Render(spriteBatch);
            }
        }

        /// <summary>
        ///     Event handler for resizing the window.
        /// </summary>
        protected abstract void Window_ClientSizeChanged(object sender, System.EventArgs e);

        /// <summary>
        ///     Perform the update for this frame.
        /// </summary>
        public virtual void Update()
        {
            foreach (var child in _updateableChildren)
            {
                child.Update();
            }
        }

        public void AddChild(IRenderableElement element)
        {
            _children.Add(element);
            if (element is IUpdateableElement)
            {
                _updateableChildren.Add(element as IUpdateableElement);
            }
        }

        public void RemoveChild(IRenderableElement element)
        {
            _children.Remove(element);
            _updateableChildren.Remove(element as IUpdateableElement);
        }
    }
}
