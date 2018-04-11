using Orbis.Simulation;
using System.Collections.Generic;

namespace Orbis.World
{
    /// <summary>
    /// Author: Bram Kelder
    /// </summary>
    public class Scene
    {
        public Map WorldMap { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public int Seed { get; private set; }
        public BiomeCollection BiomeCollection { get; private set; }

        public XMLModel.WorldSettings Settings { get; private set; }
        public XMLModel.DecorationCollection DecorationSettings { get; private set; }

        public Scene(int seed, XMLModel.WorldSettings settings, XMLModel.DecorationCollection decorationCollection, BiomeCollection biomeCollection)
        {
            Seed = seed;
            Settings = settings;
            DecorationSettings = decorationCollection;
            BiomeCollection = biomeCollection;
        }
    }
}
