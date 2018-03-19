using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    /// <summary>
    /// Manages cell decoration meshes
    /// </summary>
    class DecorationManager
    {
        private XMLModel.DecorationCollection data;
        private Dictionary<string, RenderableMesh> nameMeshMap;

        public DecorationManager(XMLModel.DecorationCollection collection, AtlasModelLoader modelLoader, GraphicsDevice device)
        {
            data = collection;
            nameMeshMap = new Dictionary<string, RenderableMesh>();
            foreach(var decoration in data.Decorations)
            {
                nameMeshMap.Add(decoration.Name, new RenderableMesh(device,
                    modelLoader.LoadModel(decoration.Model.Name, decoration.Model.Texture, decoration.Model.ColorTexture).Mesh));
            }
        }

        /// <summary>
        /// Gets the mesh of the decoration with the given name.
        /// </summary>
        /// <param name="name">Name of the decoration</param>
        /// <returns>Mesh or null</returns>
        public RenderableMesh GetDecorationMesh(string name)
        {
            RenderableMesh mesh;
            nameMeshMap.TryGetValue(name, out mesh);
            return mesh;
        }
    }
}
