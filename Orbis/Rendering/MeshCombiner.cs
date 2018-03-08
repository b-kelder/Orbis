using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
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

        public void Add(MeshInstance instance)
        {
            if(instance.mesh.VertexCount + currentVertexCount > ushort.MaxValue)
            {
                Combine();
            }
            instances.Add(instance);
            currentVertexCount += instance.mesh.VertexCount;
        }

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
