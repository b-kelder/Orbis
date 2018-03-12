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
    /// Contains basic mesh data
    /// </summary>
    class Mesh
    {
        private bool dirtyFlag;
        private Vector3[] vertices;
        private Vector2[] uvs;
        private ushort[] triangles;

        public Vector3[] Vertices {
            get
            {
                return vertices;
            }
            set
            {
                if(vertices != value)
                {
                    dirtyFlag = true;
                    vertices = value;
                }
            }
        }
        public Vector2[] UVs
        {
            get
            {
                return uvs;
            }
            set
            {
                if(uvs != value)
                {
                    dirtyFlag = true;
                    uvs = value;
                }
            }
        }
        public ushort[] Triangles
        {
            get
            {
                return triangles;
            }
            set
            {
                if(triangles != value)
                {
                    dirtyFlag = true;
                    triangles = value;
                }
            }
        }
        public int VertexCount { get { return Vertices.Length; } }
        public int TriangleCount { get { return Triangles.Length / 3; } }
        public bool Dirty { get { return dirtyFlag; } }
        public RenderableMesh RenderableMesh { get; set; }

        public Mesh()
        {
            Vertices = new Vector3[0];
            UVs = new Vector2[0];
            Triangles = new ushort[0];
        }

        public Mesh(IEnumerable<MeshInstance> meshes)
        {
            CombineMeshes(meshes);
        }

        public VertexBuffer CreateVertexBuffer(GraphicsDevice device)
        {
            if(UVs.Length != Vertices.Length)
            {
                throw new InvalidOperationException("Vertices and UV must be of the same length");
            }
            VertexPositionTexture[] vertexData = new VertexPositionTexture[Vertices.Length];
            for(int i = 0; i < vertexData.Length; i++)
            {
                vertexData[i].Position = Vertices[i];
                vertexData[i].TextureCoordinate = UVs[i];
            }
            var vb = new VertexBuffer(device, typeof(VertexPositionTexture), vertexData.Length, BufferUsage.WriteOnly);
            vb.SetData(vertexData);
            return vb;
        }

        public IndexBuffer CreateIndexBuffer(GraphicsDevice device)
        {
            var ib = new IndexBuffer(device, IndexElementSize.SixteenBits, Triangles.Length, BufferUsage.WriteOnly);
            ib.SetData(Triangles);
            return ib;
        }

        public void MakeRenderable(GraphicsDevice device)
        {
            if(Dirty)
            {
                // TODO: No idea if this is the correct course of action
                if(RenderableMesh != null)
                {
                    RenderableMesh.Dispose();
                }
                RenderableMesh = new RenderableMesh(device, this);
                dirtyFlag = false;
            }
        }

        private void CombineMeshes(IEnumerable<MeshInstance> meshes)
        {
            // TODO: Optimize
            var vertexList = new List<Vector3>();
            var uvList = new List<Vector2>();
            var triangleList = new List<ushort>();

            foreach(var mesh in meshes)
            {
                int vertOffset = vertexList.Count;
                if(vertOffset + mesh.mesh.Vertices.Length > ushort.MaxValue)
                {
                    throw new IndexOutOfRangeException("Combined vertex count exceeds max of 65535");
                }

                foreach(var vert in mesh.mesh.Vertices)
                {
                    vertexList.Add(Vector3.Transform(vert, mesh.matrix));
                }
                uvList.AddRange(mesh.mesh.UVs);
                foreach(var index in mesh.mesh.Triangles)
                {
                    triangleList.Add((ushort)(index + vertOffset));
                }
            }

            this.Vertices = vertexList.ToArray();
            this.UVs = uvList.ToArray();
            this.Triangles = triangleList.ToArray();
        }
    }

    struct MeshInstance
    {
        public Mesh mesh;
        public Matrix matrix;
        public Point pos;
    }
}
