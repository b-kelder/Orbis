using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Engine
{
    public class Entity
    {
        public Texture2D Texture { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public bool Visible { get; set; }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void LoadContent(ContentManager content)
        {

        }

        public virtual void UnloadContent()
        {

        }
    }
}
