using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// An automatically expanding pool of cell decorations.
    /// </summary>
    class DecorationPool
    {
        /// <summary>
        /// Inidicates if an index is occupied
        /// </summary>
        private List<bool> occupation;
        /// <summary>
        /// The basic decoration mesh
        /// </summary>
        private Mesh baseMesh;
        /// <summary>
        /// Combined renderable meshes
        /// </summary>
        private List<RenderableMesh> combinedRenderableMesh;
        /// <summary>
        /// Indicates which combinedRenderableMesh indexes need updates
        /// </summary>
        private List<bool> shouldBeUpdated;
        /// <summary>
        /// The graphics device
        /// </summary>
        private GraphicsDevice device;
        /// <summary>
        /// Amount of base meshes that can be combined in a single combi mesh
        /// </summary>
        private int meshesPerCombi;
        /// <summary>
        /// Random used for rotation randomization
        /// </summary>
        private Random random;

        /// <summary>
        /// Creates a new DecorationPool for the given mesh.
        /// </summary>
        /// <param name="mesh">The decoration mesh</param>
        /// <param name="device">The graphics device to use</param>
        /// <param name="startMeshCount">The amount of combined meshes to start with</param>
        public DecorationPool(Mesh mesh, GraphicsDevice device, int startMeshCount)
        {
            this.device = device;
            baseMesh = mesh;
            combinedRenderableMesh = new List<RenderableMesh>();
            occupation = new List<bool>();
            shouldBeUpdated = new List<bool>();
            meshesPerCombi = ushort.MaxValue / mesh.VertexCount;

            for (int i = 0; i < startMeshCount; i++)
            {
                AddCombiMesh();
            }

            random = new Random(mesh.TriangleCount);
        }

        /// <summary>
        /// Adds a new combined mesh
        /// </summary>
        private void AddCombiMesh()
        {
            var instances = new List<MeshInstance>();
            for (int i = 0; i < meshesPerCombi; i++)
            {
                instances.Add(new MeshInstance
                {
                    mesh = baseMesh,
                    matrix = Matrix.Identity,
                    tag = i,
                });
            }
            shouldBeUpdated.Add(true);
            combinedRenderableMesh.Add(new RenderableMesh(device, new Mesh(instances)));
            for(int i = 0; i < meshesPerCombi; i++)
            {
                occupation.Add(false);
            }
        }

        /// <summary>
        /// Gets a free decoration index
        /// </summary>
        /// <returns>An index</returns>
        public int GetFreeIndex()
        {
            for (int i = 0; i < occupation.Count; i++)
            {
                if (occupation[i] == false)
                {
                    occupation[i] = true;
                    return i;
                }
            }
            // Add another combi mesh
            AddCombiMesh();
            return GetFreeIndex();
        }

        /// <summary>
        /// Frees a given index
        /// </summary>
        /// <param name="index">Index to free</param>
        public void FreeIndex(int index)
        {
            if (index >= 0 && index < occupation.Count)
            {
                occupation[index] = false;
                SetPosition(index, Vector3.Zero);
            }
        }

        /// <summary>
        /// Sets the position of a decoration
        /// </summary>
        /// <param name="index">The decoration's index</param>
        /// <param name="position">Position to set it to</param>
        public void SetPosition(int index, Vector3 position)
        {
            if (index < 0 || index >= occupation.Count)
            {
                throw new IndexOutOfRangeException();
            }

            // Set vertexes to base mesh + offset and add a random rotation
            int meshIndex = index / this.meshesPerCombi;
            int offset = (index - meshIndex * this.meshesPerCombi) * baseMesh.VertexCount;
            var matrix = Matrix.CreateRotationZ(MathHelper.ToRadians((float)random.NextDouble() * 360)) *
                Matrix.CreateTranslation(position);
            for (int i = 0; i < baseMesh.VertexCount; i++)
            {
                combinedRenderableMesh[meshIndex].VertexData[i + offset].Position = Vector3.Transform(baseMesh.Vertices[i], matrix);
            }
            shouldBeUpdated[meshIndex] = true;
        }

        /// <summary>
        /// Adds meshes that should be updated to the set.
        /// </summary>
        /// <param name="updateSet">The set to add to</param>
        public void Update(HashSet<RenderableMesh> updateSet)
        {
            for (int i = 0; i < shouldBeUpdated.Count; i++)
            {
                if (shouldBeUpdated[i])
                {
                    updateSet.Add(combinedRenderableMesh[i]);
                    shouldBeUpdated[i] = false;
                }
            }
        }

        /// <summary>
        /// Returns a list of render instances for this pool's meshes.
        /// </summary>
        /// <returns>List of RenderInstances</returns>
        public List<RenderInstance> GetActiveInstances()
        {
            var instances = new List<RenderInstance>();
            for (int i = 0; i < this.combinedRenderableMesh.Count; i++)
            {
                // Just returning all of them instead of checking occupation is actually more performant
                // Perhaps caching the occupation on a per-mesh basis would give better performance
                /*bool render = false;
                for (int k = 0; k < this.meshesPerCombi; k++)
                {
                    if (this.occupation[i * this.meshesPerCombi + k] == true)
                    {
                        render = true;
                        break;
                    }
                }
                if (render)*/
                {
                    instances.Add(new RenderInstance()
                    {
                        mesh = combinedRenderableMesh[i],
                        matrix = Matrix.Identity,
                    });
                }
            }
            return instances;
        }
    }
}
