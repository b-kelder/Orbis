using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Combines arbitrary meshes into 1 or more meshes.
    /// </summary>
    class MeshCombiner
    {
        private List<Mesh> combinedMeshes;
        private int currentVertexCount;
        private List<MeshInstance> instances;

        public MeshCombiner()
        {
            currentVertexCount = 0;
            combinedMeshes = new List<Mesh>();
            instances = new List<MeshInstance>();
        }

        /// <summary>
        /// Adds a mesh to the combiner and returns the mesh it will be part of.
        /// </summary>
        /// <param name="instance">Instance to add</param>
        /// <returns>Combined mesh index it will be combined into</returns>
        public int Add(MeshInstance instance)
        {
            int index = 0;
            if(instance.mesh.VertexCount + currentVertexCount > ushort.MaxValue)
            {
                Combine();
                index = combinedMeshes.Count - 1;
            }
            instances.Add(instance);
            currentVertexCount += instance.mesh.VertexCount;
            index = combinedMeshes.Count;
            return index;
        }

        /// <summary>
        /// Returns the combined meshes.
        /// </summary>
        /// <returns>List of combined meshes</returns>
        public List<Mesh> GetCombinedMeshes()
        {
            if(instances.Count > 0)
            {
                Combine();
            }
            return combinedMeshes;
        }

        private void Combine()
        {
            combinedMeshes.Add(new Mesh(instances));
            instances.Clear();
            currentVertexCount = 0;
        }
    }
}
