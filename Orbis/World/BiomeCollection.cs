using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbis.World
{
    public class BiomeCollection
    {
        private Dictionary<Color, Biome> biomeColorMap;
        private Dictionary<string, Biome> biomeNameMap;
        private Color[] landBiomes;
        private Color[] seaBiomes;
        private int mapStride;

        public BiomeCollection(XMLModel.BiomeCollection biomeData, ContentManager contentManager)
        {
            biomeColorMap = new Dictionary<Color, Biome>();
            biomeNameMap = new Dictionary<string, Biome>();

            var texLand = contentManager.Load<Texture2D>(biomeData.LandBiomeMap);
            var texSea = contentManager.Load<Texture2D>(biomeData.SeaBiomeMap);

            if(texLand.Width != texSea.Width || texLand.Height != texSea.Height || texLand.Height != 32 || texLand.Width != 128)
            {
                throw new Exception("Biome maps have wrong sizes!");
            }

            landBiomes = new Color[texLand.Width * texLand.Height];
            seaBiomes = new Color[texSea.Width * texSea.Height];
            mapStride = texLand.Width;

            texLand.GetData(landBiomes);
            texSea.GetData(seaBiomes);

            foreach(var biome in biomeData.Biomes)
            {
                if(!AddBiome(biome))
                {
                    throw new Exception("Duplicate biome names or colors!");
                }
            }
        }

        public bool AddBiome(XMLModel.Biome biomeData)
        {
            var color = new Color(biomeData.Color.R, biomeData.Color.G, biomeData.Color.B);
            if(biomeColorMap.ContainsKey(color) || biomeNameMap.ContainsKey(biomeData.Name))
            {
                return false;
            }
            var biome = new Biome(biomeData);
            biomeColorMap.Add(color, biome);
            biomeNameMap.Add(biome.Name, biome);
            return true;
        }

        public Biome GetBiome(string name)
        {
            Biome b;
            biomeNameMap.TryGetValue(name, out b);
            return b;
        }

        public Biome GetBiome(Color color)
        {
            Biome b;
            biomeColorMap.TryGetValue(color, out b);
            return b;
        }

        public Color GetLandColor(int x, int y)
        {
            return landBiomes[x + y * mapStride];
        }

        public Color GetSeaColor(int x, int y)
        {
            return seaBiomes[x + y * mapStride];
        }

        public Biome GetLandBiome(int x, int y)
        {
            return GetBiome(GetLandColor(x, y));
        }

        public Biome GetSeaBiome(int x, int y)
        {
            return GetBiome(GetSeaColor(x, y));
        }
    }
}
