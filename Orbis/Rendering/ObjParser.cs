using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Basic Wavefront .OBJ file format parser. Currently only supports:
    /// - 1 object
    /// - Triangulated faces
    /// - Vertex position
    /// - 1 UV channel
    /// </summary>
    static class ObjParser
    {
        struct FaceData
        {
            public int vertIndex;
            public int uvIndex;
        }

        /// <summary>
        /// Creates a mesh from an OBJ file stream.
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <returns>Mesh</returns>
        public static Mesh FromStream(System.IO.Stream stream)
        {
            var objVerts = new List<Vector3>();
            var objUvs = new List<Vector2>();
            var objFaces = new List<FaceData>();

            using(StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                char[] splitters = new char[] { ' ' };
                char[] faceSplitters = new char[] { '/' };

                while((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    var splits = line.Split(splitters);
                    if(line.StartsWith("v "))
                    {
                        // Vertex
                        objVerts.Add(new Vector3(
                            float.Parse(splits[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture),
                            float.Parse(splits[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture),
                            float.Parse(splits[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    else if(line.StartsWith("vt "))
                    {
                        // UV
                        objUvs.Add(new Vector2(
                            float.Parse(splits[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture),
                            1.0f - float.Parse(splits[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    else if(line.StartsWith("f "))
                    {
                        // Face
                        for(int i = 1; i <= 3; i++)
                        {
                            var indexes = splits[4 - i].Split(faceSplitters);
                            var face = new FaceData
                            {
                                // OBJ Indexes start at 1 so subtract it for 0 based indexing
                                vertIndex = int.Parse(indexes[0]) - 1,
                                uvIndex = int.Parse(indexes[1]) - 1
                            };
                            objFaces.Add(face);
                        }
                    }
                }
            }

            if(objVerts.Count > ushort.MaxValue)
            {
                throw new IndexOutOfRangeException("Too many vertices in mesh, yell at dev to auto split meshes in this case!");
            }

            // Since we can only have a 1:1 relation between our vertices and uvs we must
            // turn every unique combination of vertex and uv into an actual vertex/uv pair
            var faceDict = new Dictionary<FaceData, ushort>();
            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<ushort>();
            var colors = new List<Color>();
            foreach(var face in objFaces)
            {
                if(faceDict.ContainsKey(face))
                {
                    // We already have an index stored for this pair, use it
                    triangles.Add(faceDict[face]);
                }
                else
                {
                    // New pairing, add new vertex and uv and map it
                    ushort index = (ushort)vertices.Count;
                    triangles.Add(index);
                    vertices.Add(objVerts[face.vertIndex]);
                    uvs.Add(objUvs[face.uvIndex]);
                    //TODO: Load actual vertex color
                    colors.Add(Color.Black);
                    faceDict.Add(face, index);
                }
            }

            return new Mesh
            {
                Vertices = vertices.ToArray(),
                UVs = uvs.ToArray(),
                UVs2 = uvs.ToArray(),
                Triangles = triangles.ToArray(),
                Colors = colors.ToArray(),
            };
        }
    }
}
