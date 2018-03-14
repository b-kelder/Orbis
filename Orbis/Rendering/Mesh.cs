﻿using Microsoft.Xna.Framework;
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
        public Dictionary<object, List<int>> TagIndexMap { get; private set; }

        private bool dirtyFlag;
        private Vector3[] vertices;
        private Vector2[] uvs;
        private Vector2[] uvs2;
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
        public Vector2[] UVs2
        {
            get
            {
                return uvs2;
            }
            set
            {
                if (uvs2 != value)
                {
                    dirtyFlag = true;
                    uvs2 = value;
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
            UVs2 = new Vector2[0];
            Triangles = new ushort[0];
            Colors = new Color[0];
        }

        public Mesh(IEnumerable<MeshInstance> meshes)
        {
            CombineMeshes(meshes);
        }

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
            // Point Index Map is only possible here
            var indexMap = new Dictionary<object, List<int>>();
            // TODO: Optimize
            var vertexList = new List<Vector3>();
            var uvList = new List<Vector2>();
            var uv2List = new List<Vector2>();
            var triangleList = new List<ushort>();
            var colorList = new List<Color>();

            foreach(var mesh in meshes)
            {
                // Use points as tag
                if(!indexMap.ContainsKey(mesh.pos))
                {
                    indexMap.Add(mesh.pos, new List<int>());
                }

                int vertOffset = vertexList.Count;
                if(vertOffset + mesh.mesh.Vertices.Length > ushort.MaxValue)
                {
                    throw new IndexOutOfRangeException("Combined vertex count exceeds max of 65535");
                }

                foreach(var vert in mesh.mesh.Vertices)
                {
                    // Add tag
                    indexMap[mesh.pos].Add(vertexList.Count);
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
                    // Use instance color
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
