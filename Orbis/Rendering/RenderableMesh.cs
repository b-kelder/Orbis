using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    class RenderableMesh
    {
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }

        public RenderableMesh(GraphicsDevice device, Mesh mesh)
        {
            VertexBuffer = mesh.CreateVertexBuffer(device);
            IndexBuffer = mesh.CreateIndexBuffer(device);
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
