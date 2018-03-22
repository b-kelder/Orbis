using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// A mesh that is ready for GPU rendering.
    /// </summary>
    class RenderableMesh
    {
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public CustomVertexData[] VertexData { get; set; }

        /// <summary>
        /// Creates a new Renderable Mesh.
        /// </summary>
        /// <param name="device">The graphics device to use</param>
        /// <param name="mesh">The mesh to use</param>
        public RenderableMesh(GraphicsDevice device, Mesh mesh)
        {
            VertexData = mesh.ToVertexData(device);
            VertexBuffer = new VertexBuffer(device, typeof(CustomVertexData), VertexData.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(VertexData);
            IndexBuffer = mesh.CreateIndexBuffer(device);
        }

        /// <summary>
        /// Updates the VertexBuffer from the current state of CustomVertexData.
        /// </summary>
        public void UpdateVertexBuffer()
        {
            if(VertexBuffer.VertexCount != VertexData.Length)
            {
                throw new NotImplementedException();
                // TODO: Use this?
                //VertexBuffer.Dispose();
                //VertexBuffer = new VertexBuffer(device, typeof(CustomVertexData), VertexData.Length, BufferUsage.WriteOnly);
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
