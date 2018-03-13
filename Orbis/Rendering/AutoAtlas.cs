using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Orbis.Rendering
{
    class AutoAtlas
    {
        private Dictionary<Texture2D, Matrix> textureUVOffset;
        private Dictionary<Texture2D, Point> textureAtlasOffset;
        private Texture2D atlas;
        private int width;
        private int height;

        private int currentFreeX;
        private int currentFreeY;
        private int nextFreeRowY;

        public Texture2D Texture { get { return atlas; } }

        public AutoAtlas(int width, int height)
        {
            this.width = width;
            this.height = height;
            textureUVOffset = new Dictionary<Texture2D, Matrix>();
            textureAtlasOffset = new Dictionary<Texture2D, Point>();
        }

        public void AddTexture(Texture2D texture)
        {
            if(textureUVOffset.ContainsKey(texture))
            {
                return;
            }

            if(!TryAddTexture(texture))
            {
                throw new Exception("Atlas has run out of space");
            }
        }

        private bool TryAddTexture(Texture2D texture)
        {
            if(currentFreeX + texture.Width <= width)
            {
                if(currentFreeY + texture.Height <= height)
                {
                    // fits!
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
                // Try next row
                currentFreeX = 0;
                currentFreeY = nextFreeRowY;
                if (currentFreeY + texture.Height <= height)
                {
                    // fits!
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
            textureAtlasOffset.Add(texture, new Point(currentFreeX, currentFreeY));

            var matrix = Matrix.CreateScale((float)texture.Width / width, (float)texture.Height / height, 1) *
                Matrix.CreateTranslation((float)currentFreeX / width, (float)currentFreeY / height, 0);
            textureUVOffset.Add(texture, matrix);

            if (currentFreeY + texture.Height > nextFreeRowY)
            {
                nextFreeRowY = currentFreeY + texture.Height;
            }
            currentFreeX += texture.Width;
        }

        public Matrix GetOffset(Texture2D texture)
        {
            Matrix result;
            textureUVOffset.TryGetValue(texture, out result);
            return result;
        }

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

        private VertexPositionColorTexture[] CreateQuad()
        {
            var floorVerts = new VertexPositionColorTexture[6];

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

            return floorVerts;
        }

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
            var quad = CreateQuad();
            device.SetRenderTarget(renderTarget);

            device.Clear(Color.Magenta);

            // Now render a quad per texture
            foreach(var data in textureUVOffset)
            { 
                shader.Texture = data.Key;
                shader.TextureEnabled = true;
                shader.World = data.Value;

                foreach (var pass in shader.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    device.DrawUserPrimitives(PrimitiveType.TriangleList, quad, 0, 2);
                }
            }

            // Restore device state
            device.SetRenderTargets(oldRenderTarget);
            device.BlendFactor = oldBlendFactor;
            device.BlendState = oldBlendState;
            device.DepthStencilState = oldStencilState;

            //
            //shader.Dispose();
            atlas = renderTarget;
        }

        public void UnloadNonAtlasTextures()
        {
            foreach (var tex in textureUVOffset.Keys)
            {
                tex.Dispose();
            }
        }
    }
}
