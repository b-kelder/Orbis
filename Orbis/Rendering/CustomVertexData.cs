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
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration => CustomVertexData.VertexDeclaration;

        Vector3 _position;
        Vector2 _texCoord0;
        Vector2 _texCoord1;
        Color _color;

        public Vector3 Position { get { return _position; } set { _position = value; } }
        public Vector2 TextureCoordinate0 { get { return _texCoord0; } set { _texCoord0 = value; } }
        public Vector2 TextureCoordinate1 { get { return _texCoord1; } set { _texCoord1 = value; } }
        public Color Color { get { return _color; } set { _color = value; } }
    }
}
