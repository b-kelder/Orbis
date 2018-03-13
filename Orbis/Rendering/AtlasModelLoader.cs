using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Rendering
{
    class AtlasModelLoader
    {
        private AutoAtlas atlas;
        private AutoAtlas colorAtlas;
        private Effect shader;
        private ContentManager contentManager;
        private Material material;

        public string BaseModelDirectory { get; set; }
        public string BaseTextureDirectory { get; set; }

        public AtlasModelLoader(int atlasWidth, int atlasHeight, Effect shader, ContentManager contentManager)
        {
            atlas = new AutoAtlas(atlasWidth, atlasHeight);
            colorAtlas = new AutoAtlas(atlasWidth, atlasHeight);
            this.shader = shader;
            this.contentManager = contentManager;
            this.material = new Material(shader);

            BaseModelDirectory = "Meshes";
            BaseTextureDirectory = "Textures";
        }

        public Model LoadModel(string meshFile, string textureBaseName)
        {
            return LoadModel(meshFile, textureBaseName, textureBaseName + "_c");
        }

        public Model LoadModel(string meshFile, string diffuseName, string colorName)
        {
            Mesh mesh = null;
            using (var stream = TitleContainer.OpenStream("Content/" + BaseModelDirectory + "/" + meshFile + ".obj"))
            {
                mesh = ObjParser.FromStream(stream);
            }
            var diffuseTexture = TryLoadTexture(BaseTextureDirectory + "/" + diffuseName);
            var colorTexture = TryLoadTexture(BaseTextureDirectory + "/" + colorName);
            if(diffuseTexture == null || colorTexture == null)
            {
                throw new Exception("Need both a color and diffuse texture!");
            }
            if(diffuseTexture.Width != colorTexture.Width || diffuseTexture.Height != colorTexture.Height)
            {
                throw new Exception("Color and diffuse texture must be the same size!");
            }
            // These should stay in sync if the resolution is the same
            atlas.AddTexture(diffuseTexture);
            colorAtlas.AddTexture(colorTexture);
            atlas.UpdateMeshUVs(mesh, diffuseTexture);

            return new Model(mesh, material);
        }

        private Texture2D TryLoadTexture(string name)
        {
            try
            {
                return contentManager.Load<Texture2D>(name);
            }
            catch(ContentLoadException e)
            {
                Debug.WriteLine("Exception loading " + name + ": " + e.Message);
                return null;
            }
        }

        public void FinializeLoading(GraphicsDevice graphicsDevice)
        {
            atlas.Create(graphicsDevice);
            colorAtlas.Create(graphicsDevice);

            material.Texture = atlas.Texture;
            material.ColorMap = colorAtlas.Texture;
        }
    }
}
