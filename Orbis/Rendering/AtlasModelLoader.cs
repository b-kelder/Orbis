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
    /// <summary>
    /// Loads Models and automatically combines their textures into texture atlasses.
    /// </summary>
    class AtlasModelLoader
    {
        private AutoAtlas atlas;
        private AutoAtlas colorAtlas;
        private Effect shader;
        private ContentManager contentManager;
        private Material material;

        public string BaseModelDirectory { get; set; }
        public string BaseTextureDirectory { get; set; }
        public Material Material { get { return material; } }


        public AtlasModelLoader(int atlasWidth, int atlasHeight, Effect shader, ContentManager contentManager)
        {
            atlas = new AutoAtlas(atlasWidth, atlasHeight, 32);
            colorAtlas = new AutoAtlas(atlasWidth, atlasHeight, 32);
            this.shader = shader;
            this.contentManager = contentManager;
            this.material = new Material(shader);

            BaseModelDirectory = "Meshes";
            BaseTextureDirectory = "Textures";
        }

        /// <summary>
        /// Loads a model using the given model file and texture base name. Color map will be
        /// textureBaseName_c
        /// </summary>
        /// <param name="meshFile">Mesh to load</param>
        /// <param name="textureBaseName">Texture to load</param>
        /// <returns>Model that is set up to use the atlas</returns>
        public Model LoadModel(string meshFile, string textureBaseName)
        {
            return LoadModel(meshFile, textureBaseName, textureBaseName + "_c");
        }

        /// <summary>
        /// Loads a model using the given model file and texture files.
        /// </summary>
        /// <param name="meshFile">Mesh to load</param>
        /// <param name="textureBaseName">Texture to load</param>
        /// <param name="colorName">Color map texture to load</param>
        /// <returns>Model that is set up to use the atlas</returns>
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
            // Color and Diffuse UVs are seperate
            atlas.AddTexture(diffuseTexture);
            colorAtlas.AddTexture(colorTexture);
            atlas.UpdateMeshUVs(mesh, diffuseTexture, 0);
            colorAtlas.UpdateMeshUVs(mesh, colorTexture, 1);

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

        /// <summary>
        /// Finalizes loading by generating atlasses and unloading textures.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public void FinializeLoading(GraphicsDevice graphicsDevice)
        {
            atlas.Create(graphicsDevice);
            colorAtlas.Create(graphicsDevice);

            material.Texture = atlas.Texture;
            material.ColorMap = colorAtlas.Texture;

            atlas.UnloadNonAtlasTextures();
            colorAtlas.UnloadNonAtlasTextures();
        }
    }
}
