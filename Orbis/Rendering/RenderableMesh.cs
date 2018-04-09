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
        /// <summary>
        /// This mesh's vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer { get; set; }
        /// <summary>
        /// This mesh's index buffer
        /// </summary>
        public IndexBuffer IndexBuffer { get; set; }
        /// <summary>
        /// Vertex data in an editable format
        /// </summary>
        public CustomVertexData[] VertexData { get; private set; }

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
            VertexBuffer.SetData(VertexData);
        }

        /// <summary>
        /// Disposes any used graphic resources.
        /// </summary>
        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
