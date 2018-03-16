using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
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

        public RenderableMesh GetDecorationMesh(string name)
        {
            RenderableMesh mesh;
            nameMeshMap.TryGetValue(name, out mesh);
            return mesh;
        }
    }
}
