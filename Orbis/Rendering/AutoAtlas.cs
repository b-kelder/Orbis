using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbis.Rendering
{
    /// <summary>
    /// Automated texture atlas for combining textures into a single large texture.
    /// </summary>
    class AutoAtlas
    {
        private Dictionary<Texture2D, Matrix> textureUVOffset;
        private Dictionary<Texture2D, Point> textureAtlasOffset;
        private int width;
        private int height;

        // Variables for placing new textures in the atlas.
        // The current implementation uses a simple row based approach where each row's height
        // is the tallest texture in that row. This wastes some space when textures vary a lot
        // in size but it works for our use case.
        private int border;
        private int currentFreeX;
        private int currentFreeY;
        private int nextFreeRowY;

        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Creates a new atlas.
        /// </summary>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="border">Border around a texture in pixels</param>
        public AutoAtlas(int width, int height, int border)
        {
            this.width = width;
            this.height = height;
            this.border = border;
            textureUVOffset = new Dictionary<Texture2D, Matrix>();
            textureAtlasOffset = new Dictionary<Texture2D, Point>();
        }

        /// <summary>
        /// Adds a new texture to the atlas.
        /// </summary>
        /// <param name="texture">Texture to add</param>
        /// <exception cref="Exception">Thrown when atlas is full</exception>
        public void AddTexture(Texture2D texture)
        {
            // Don't add duplicates
            if(textureUVOffset.ContainsKey(texture))
            {
                return;
            }

            if(!TryAddTexture(texture))
            {
                throw new Exception("Atlas has run out of space");
            }
        }

        /// <summary>
        /// Tries to add a texture to the atlas
        /// </summary>
        /// <param name="texture">The texture to add</param>
        /// <returns>Success</returns>
        private bool TryAddTexture(Texture2D texture)
        {
            int texWidth = texture.Width + border * 2;
            int texHeight = texture.Height + border * 2;

            if(currentFreeX + texWidth <= width)
            {
                // Current row is available
                if(currentFreeY + texHeight <= height)
                {
                    // It fits
                    AddTexForCurrent(texture);
                    return true;
                }
                else
                {
                    // Atlas is full
                    return false;
                }
            }
            else
            {
                // Try the next row
                currentFreeX = 0;
                currentFreeY = nextFreeRowY;
                if (currentFreeY + texHeight <= height)
                {
                    // It fits
                    AddTexForCurrent(texture);
                    return true;
                }
                else
                {
                    // Atlas is full
                    return false;
                }
            }
        }

        private void AddTexForCurrent(Texture2D texture)
        {
            int texWidth = texture.Width + border * 2;
            int texHeight = texture.Height + border * 2;

            textureAtlasOffset.Add(texture, new Point(currentFreeX, currentFreeY));
            // Create UV matrix by scaling and offsetting based on texture size and location in atlas
            var matrix = Matrix.CreateScale((float)texture.Width / width, (float)texture.Height / height, 1) *
                Matrix.CreateTranslation((float)(currentFreeX + border) / width, (float)(currentFreeY + border) / height, 0);
            textureUVOffset.Add(texture, matrix);

            // Update next free Y coordinate if this texture is higher than anything on this row
            if (currentFreeY + texHeight > nextFreeRowY)
            {
                nextFreeRowY = currentFreeY + texHeight;
            }
            currentFreeX += texWidth;
        }

        /// <summary>
        /// Gets the UV matrix for the given texture inside the atlas.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public Matrix GetOffset(Texture2D texture)
        {
            Matrix result;
            textureUVOffset.TryGetValue(texture, out result);
            return result;
        }

        /// <summary>
        /// Updates the UVs of a mesh for the given texture.
        /// </summary>
        /// <param name="mesh">The mesh to update</param>
        /// <param name="texture">Texture to map for</param>
        /// <param name="uvMapIndex">UV map to modify (0 or 1)</param>
        public void UpdateMeshUVs(Mesh mesh, Texture2D texture, int uvMapIndex)
        {
            var matrix = GetOffset(texture);
            if(matrix == Matrix.Identity)
            {
                return;
            }

            if(uvMapIndex == 0)
            {
                for (int i = 0; i < mesh.UVs.Length; i++)
                {
                    var newUv = Vector2.Transform(mesh.UVs[i], matrix);
                    newUv.Y = 1 - newUv.Y;
                    mesh.UVs[i] = newUv;
                }
            }
            else if(uvMapIndex == 1)
            {
                for (int i = 0; i < mesh.UVs2.Length; i++)
                {
                    var newUv = Vector2.Transform(mesh.UVs2[i], matrix);
                    newUv.Y = 1 - newUv.Y;
                    mesh.UVs2[i] = newUv;
                }
            }
        }

        /// <summary>
        /// Creates a quad with borders used for rendering a texture to an atlas.
        /// </summary>
        /// <param name="borderSize">Relative border size [0-1]</param>
        /// <returns></returns>
        private VertexPositionColorTexture[] CreateQuad(float borderSize)
        {
            var floorVerts = new VertexPositionColorTexture[6 * 5];
            #region Quad data
            // Center quad
            floorVerts[0].Position = new Vector3(0, 0, 0);
            floorVerts[1].Position = new Vector3(0, 1, 0);
            floorVerts[2].Position = new Vector3(1, 0, 0);

            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(1, 1, 0);
            floorVerts[5].Position = floorVerts[2].Position;

            floorVerts[0].TextureCoordinate = new Vector2(0, 0);
            floorVerts[1].TextureCoordinate = new Vector2(0, 1);
            floorVerts[2].TextureCoordinate = new Vector2(1, 0);

            floorVerts[3].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[4].TextureCoordinate = new Vector2(1, 1);
            floorVerts[5].TextureCoordinate = floorVerts[2].TextureCoordinate;

            // Top border quad
            floorVerts[6].Position = floorVerts[1].Position;
            floorVerts[7].Position = floorVerts[1].Position + new Vector3(-borderSize, borderSize, 0);
            floorVerts[8].Position = floorVerts[4].Position;

            floorVerts[9].Position = floorVerts[4].Position;
            floorVerts[10].Position = floorVerts[7].Position;
            floorVerts[11].Position = floorVerts[4].Position + new Vector3(borderSize, borderSize, 0);

            floorVerts[6].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[7].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[8].TextureCoordinate = floorVerts[4].TextureCoordinate;

            floorVerts[9].TextureCoordinate = floorVerts[4].TextureCoordinate;
            floorVerts[10].TextureCoordinate = floorVerts[7].TextureCoordinate;
            floorVerts[11].TextureCoordinate = floorVerts[4].TextureCoordinate;

            // Right border quad
            floorVerts[12].Position = floorVerts[4].Position;
            floorVerts[13].Position = floorVerts[11].Position;
            floorVerts[14].Position = floorVerts[2].Position;

            floorVerts[15].Position = floorVerts[2].Position;
            floorVerts[16].Position = floorVerts[11].Position;
            floorVerts[17].Position = floorVerts[2].Position + new Vector3(borderSize, -borderSize, 0);

            floorVerts[12].TextureCoordinate = floorVerts[4].TextureCoordinate;
            floorVerts[13].TextureCoordinate = floorVerts[11].TextureCoordinate;
            floorVerts[14].TextureCoordinate = floorVerts[2].TextureCoordinate;

            floorVerts[15].TextureCoordinate = floorVerts[2].TextureCoordinate;
            floorVerts[16].TextureCoordinate = floorVerts[11].TextureCoordinate;
            floorVerts[17].TextureCoordinate = floorVerts[2].TextureCoordinate;

            // Bottom border quad
            floorVerts[18].Position = floorVerts[2].Position;
            floorVerts[19].Position = floorVerts[17].Position;
            floorVerts[20].Position = floorVerts[0].Position;

            floorVerts[21].Position = floorVerts[0].Position;
            floorVerts[22].Position = floorVerts[17].Position;
            floorVerts[23].Position = floorVerts[0].Position + new Vector3(-borderSize, -borderSize, 0);

            floorVerts[18].TextureCoordinate = floorVerts[2].TextureCoordinate;
            floorVerts[19].TextureCoordinate = floorVerts[17].TextureCoordinate;
            floorVerts[20].TextureCoordinate = floorVerts[0].TextureCoordinate;

            floorVerts[21].TextureCoordinate = floorVerts[0].TextureCoordinate;
            floorVerts[22].TextureCoordinate = floorVerts[17].TextureCoordinate;
            floorVerts[23].TextureCoordinate = floorVerts[0].TextureCoordinate;

            // Left border quad
            floorVerts[24].Position = floorVerts[0].Position;
            floorVerts[25].Position = floorVerts[23].Position;
            floorVerts[26].Position = floorVerts[1].Position;

            floorVerts[27].Position = floorVerts[1].Position;
            floorVerts[28].Position = floorVerts[23].Position;
            floorVerts[29].Position = floorVerts[7].Position;

            floorVerts[24].TextureCoordinate = floorVerts[0].TextureCoordinate;
            floorVerts[25].TextureCoordinate = floorVerts[23].TextureCoordinate;
            floorVerts[26].TextureCoordinate = floorVerts[1].TextureCoordinate;

            floorVerts[27].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[28].TextureCoordinate = floorVerts[23].TextureCoordinate;
            floorVerts[29].TextureCoordinate = floorVerts[7].TextureCoordinate;
            #endregion
            return floorVerts;
        }

        /// <summary>
        /// Creates the atlas using the given graphics device for rendering.
        /// </summary>
        /// <param name="device">The game's graphic device</param>
        public void Create(GraphicsDevice device)
        {
            // Since GPUs are very good at doing parallel operations we will use it to create our texture atlas
            // Like multithreading, but better
            // Save device state
            var oldBlendState = device.BlendState;
            var oldBlendFactor = device.BlendFactor;
            var oldStencilState = device.DepthStencilState;
            var oldRenderTarget = device.GetRenderTargets();

            // Translates from UV to render space
            var uvToRender = Matrix.CreateScale(2) * Matrix.CreateTranslation(-1.0f, -1.0f, 0);

            // Create basic shader
            var shader = new BasicEffect(device)
            {
                TextureEnabled = false,
                View = uvToRender,
                Projection = Matrix.Identity,
            };

            // Set up render target
            var renderTarget = new RenderTarget2D(device, width, height);
            device.SetRenderTarget(renderTarget);
            device.Clear(Color.Black);

            // Now render a quad per texture
            foreach(var data in textureUVOffset)
            { 
                shader.Texture = data.Key;
                shader.TextureEnabled = true;
                shader.World = data.Value;

                var quad = CreateQuad(border / (float)data.Key.Width);

                foreach (var pass in shader.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    device.DrawUserPrimitives(PrimitiveType.TriangleList, quad, 0, quad.Length / 3);
                }
            }

            // Restore device state
            device.SetRenderTargets(oldRenderTarget);
            device.BlendFactor = oldBlendFactor;
            device.BlendState = oldBlendState;
            device.DepthStencilState = oldStencilState;

            // Clean up a bit
            shader.Dispose();
            Texture = renderTarget;
        }

        /// <summary>
        /// Unloads all textures passed to this atlas.
        /// </summary>
        public void UnloadNonAtlasTextures()
        {
            foreach (var tex in textureUVOffset.Keys)
            {
                tex.Dispose();
            }
        }
    }
}
