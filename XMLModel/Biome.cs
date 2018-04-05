
using System.Collections.Generic;

namespace XMLModel
{
    public class BiomeCollection
    {
        /// <summary>
        /// Texture name of the land biome map
        /// </summary>
        public string LandBiomeMap;
        /// <summary>
        /// Texture name of the sea biome map
        /// </summary>
        public string SeaBiomeMap;
        /// <summary>
        /// Biome list
        /// </summary>
        public List<Biome> Biomes;
    }

    public class Biome
    {
        /// <summary>
        /// Name of the biome, must be unique
        /// </summary>
        public string Name = "unknown";
        /// <summary>
        /// Color of the biome in the biome map, must be unique
        /// </summary>
        public Color Color;
        /// <summary>
        /// Modifier for the amount of food within the biome used as a basis for cell modifiers
        /// </summary>
        public double FoodModifier = 0;
        /// <summary>
        /// Modifier for the amount of recources in the biome  used as a basis for cell modifiers
        /// </summary>
        public double ResourceModifier = 0;
        /// <summary>
        /// Modifier for the amount of usable space for the population  used as a basis for cell modifiers
        /// </summary>
        public double PopulationModifier = 0;
        /// <summary>
        /// Model to use for a hex in this biome
        /// </summary>
        public Model HexModel;
        /// <summary>
        /// Default cell decoration for this biome, can be null
        /// </summary>
        public string DefaultDecoration;
        /// <summary>
        /// Default density of the decoration in this biome from 0 to 1
        /// </summary>
        public float DecorationDensity;
    }
}
