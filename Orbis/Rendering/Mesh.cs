using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbis.Rendering
{
    /// <summary>
    /// Contains basic mesh data
    /// </summary>
    class Mesh
    {
        /// <summary>
        /// Returns a map mapping tags to lists vertex indexes.
        /// The tags can only be set by the MeshInstance constructor.
        /// </summary>
        public Dictionary<object, List<int>> TagIndexMap { get; private set; }

        public Vector3[] Vertices { get; set; }
        public Vector2[] UVs { get; set; }
        public Vector2[] UVs2 { get; set; }
        public ushort[] Triangles { get; set; }
        public Color[] Colors { get; set; }
        public int VertexCount { get { return Vertices.Length; } }
        public int TriangleCount { get { return Triangles.Length / 3; } }

        /// <summary>
        /// Creates a new, empty mesh.
        /// </summary>
        public Mesh()
        {
            Vertices = new Vector3[0];
            UVs = new Vector2[0];
            UVs2 = new Vector2[0];
            Triangles = new ushort[0];
            Colors = new Color[0];
            TagIndexMap = new Dictionary<object, List<int>>();
        }

        /// <summary>
        /// Creates a mesh that combines the given mesh instances.
        /// </summary>
        /// <param name="meshes">Meshes to combine</param>
        public Mesh(IEnumerable<MeshInstance> meshes)
        {
            CombineMeshes(meshes);
        }

        /// <summary>
        /// Converts this mesh to useable vertex data.
        /// </summary>
        /// <param name="device">GraphicsDevice to use</param>
        /// <returns>CustomVertexData array</returns>
        public CustomVertexData[] ToVertexData(GraphicsDevice device)
        {
            if(UVs.Length != Vertices.Length || Vertices.Length != Colors.Length || UVs2.Length != UVs.Length)
            {
                throw new InvalidOperationException("Vertices and UV must be of the same length");
            }
            var vertexData = new CustomVertexData[Vertices.Length];
            for(int i = 0; i < vertexData.Length; i++)
            {
                vertexData[i].Position = Vertices[i];
                vertexData[i].TextureCoordinate0 = UVs[i];
                vertexData[i].TextureCoordinate1 = UVs2[i];
                vertexData[i].Color = Colors[i];
            }
            return vertexData;
        }

        /// <summary>
        /// Creates the IndexBuffer for this mesh.
        /// </summary>
        /// <param name="device">GraphicsDevice to use</param>
        /// <returns>IndexBuffer</returns>
        public IndexBuffer CreateIndexBuffer(GraphicsDevice device)
        {
            var ib = new IndexBuffer(device, IndexElementSize.SixteenBits, Triangles.Length, BufferUsage.WriteOnly);
            ib.SetData(Triangles);
            return ib;
        }

        private void CombineMeshes(IEnumerable<MeshInstance> meshes)
        {
            var indexMap = new Dictionary<object, List<int>>();
            var vertexList = new List<Vector3>();
            var uvList = new List<Vector2>();
            var uv2List = new List<Vector2>();
            var triangleList = new List<ushort>();
            var colorList = new List<Color>();

            foreach(var mesh in meshes)
            {
                int vertOffset = vertexList.Count;
                if(vertOffset + mesh.mesh.Vertices.Length > ushort.MaxValue)
                {
                    throw new IndexOutOfRangeException("Combined vertex count exceeds max of 65535");
                }

                // Vertex tagging
                if (!indexMap.ContainsKey(mesh.tag))
                {
                    indexMap.Add(mesh.tag, new List<int>());
                }
                foreach (var vert in mesh.mesh.Vertices)
                {
                    indexMap[mesh.tag].Add(vertexList.Count);
                    vertexList.Add(Vector3.Transform(vert, mesh.matrix));
                }
                uvList.AddRange(mesh.mesh.UVs);
                uv2List.AddRange(mesh.mesh.UVs2);
                foreach(var index in mesh.mesh.Triangles)
                {
                    triangleList.Add((ushort)(index + vertOffset));
                }
                if(mesh.useColor)
                {
                    // Use instance color override
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
            this.UVs2 = uv2List.ToArray();
            this.Triangles = triangleList.ToArray();
            this.Colors = colorList.ToArray();
            this.TagIndexMap = indexMap;
        }

        /// <summary>
        /// Sets the entire Mesh's color.
        /// </summary>
        /// <param name="color">Color to set to</param>
        public void SetColor(Color color)
        {
            for(int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = color;
            }
        }
    }

    /// <summary>
    /// A mesh that should be combined into a larger mesh
    /// </summary>
    struct MeshInstance
    {
        /// <summary>
        /// The mesh to use
        /// </summary>
        public Mesh mesh;
        /// <summary>
        /// Transformation matrix
        /// </summary>
        public Matrix matrix;
        /// <summary>
        /// Optional tag to later get this instance's vertices back
        /// from the combined main combi mesh
        /// </summary>
        public object tag;
        /// <summary>
        /// When true override vertex color with color
        /// </summary>
        public bool useColor;
        /// <summary>
        /// Optional vertex color override
        /// </summary>
        public Color color;
    }
}
