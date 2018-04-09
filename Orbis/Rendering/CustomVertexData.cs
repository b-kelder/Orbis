using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Stores vertex data used by our custom shader
    /// </summary>
    struct CustomVertexData : IVertexType
    {
        // Indicates location of specific vertex data inside this struct
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration => CustomVertexData.VertexDeclaration;

        public Vector3 Position { get; set; }
        public Vector2 TextureCoordinate0 { get; set; }
        public Vector2 TextureCoordinate1 { get; set; }
        public Color Color { get; set; }
    }
}
