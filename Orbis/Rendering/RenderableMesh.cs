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
        public CustomVertexData[] VertexData { get; set; }

        public RenderableMesh(GraphicsDevice device, Mesh mesh)
        {
            VertexData = mesh.ToVertexData(device);
            VertexBuffer = new VertexBuffer(device, typeof(CustomVertexData), VertexData.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(VertexData);
            IndexBuffer = mesh.CreateIndexBuffer(device);
        }

        public void UpdateVertexBuffer(GraphicsDevice device)
        {
            if(VertexBuffer.VertexCount != VertexData.Length)
            {
                throw new NotImplementedException();
                // TODO: Use this?
                VertexBuffer.Dispose();
                VertexBuffer = new VertexBuffer(device, typeof(CustomVertexData), VertexData.Length, BufferUsage.WriteOnly);
            }
            VertexBuffer.SetData(VertexData);
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
