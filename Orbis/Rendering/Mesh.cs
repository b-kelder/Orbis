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
        private Color[] colors;

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

        public Color[] Colors
        {
            get
            {
                return colors;
            }
            set
            {
                if(colors != value)
                {
                    colors = value;
                    dirtyFlag = true;
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
            Colors = new Color[0];
        }

        public Mesh(IEnumerable<MeshInstance> meshes)
        {
            CombineMeshes(meshes);
        }

        public VertexBuffer CreateVertexBuffer(GraphicsDevice device)
        {
            if(UVs.Length != Vertices.Length || Vertices.Length != Colors.Length)
            {
                throw new InvalidOperationException("Vertices and UV must be of the same length");
            }
            var vertexData = new VertexPositionColorTexture[Vertices.Length];
            for(int i = 0; i < vertexData.Length; i++)
            {
                vertexData[i].Position = Vertices[i];
                vertexData[i].TextureCoordinate = UVs[i];
                vertexData[i].Color = Colors[i];
            }
            var vb = new VertexBuffer(device, typeof(VertexPositionColorTexture), vertexData.Length, BufferUsage.WriteOnly);
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
            var colorList = new List<Color>();

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
                if(mesh.useColor)
                {
                    for(int i = 0; i < mesh.mesh.VertexCount; i++)
                    {
                        colorList.Add(mesh.color);
                    }
                }
                else
                {
                    colorList.AddRange(mesh.mesh.Colors);
                }
            }

            this.Vertices = vertexList.ToArray();
            this.UVs = uvList.ToArray();
            this.Triangles = triangleList.ToArray();
            this.Colors = colorList.ToArray();
        }

        public void AddToSelf(MeshInstance meshInstance)
        {
            if(VertexCount + meshInstance.mesh.VertexCount > ushort.MaxValue)
            {
                throw new IndexOutOfRangeException("Combined vertex count exceeds max of 65535");
            }
            var newVerts = new Vector3[vertices.Length + meshInstance.mesh.vertices.Length];
            var newUvs = new Vector2[uvs.Length + meshInstance.mesh.uvs.Length];
            var newTris = new ushort[triangles.Length + meshInstance.mesh.triangles.Length];
            var newColors = new Color[colors.Length + meshInstance.mesh.colors.Length];

            Array.Copy(vertices, newVerts, vertices.Length);
            Array.Copy(uvs, newUvs, uvs.Length);
            Array.Copy(triangles, newTris, triangles.Length);
            Array.Copy(colors, newColors, colors.Length);

            Array.Copy(meshInstance.mesh.uvs, 0, newUvs, uvs.Length, meshInstance.mesh.uvs.Length);
            Array.Copy(meshInstance.mesh.colors, 0, newColors, colors.Length, meshInstance.mesh.colors.Length);

            for(int i = 0; i < meshInstance.mesh.vertices.Length; i++)
            {
                newVerts[i + vertices.Length] = Vector3.Transform(meshInstance.mesh.vertices[i], meshInstance.matrix);
            }
            for(int i = 0; i < meshInstance.mesh.triangles.Length; i++)
            {
                newTris[i + triangles.Length] = (ushort)(meshInstance.mesh.triangles[i] + (ushort)vertices.Length);
            }

            vertices = newVerts;
            uvs = newUvs;
            triangles = newTris;
            colors = newColors;
        }

        public void SetColor(Color color)
        {
            for(int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
        }
    }

    struct MeshInstance
    {
        public Mesh mesh;
        public Matrix matrix;
        public Point pos;
        public bool useColor;
        public Color color;
    }
}
