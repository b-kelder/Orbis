using Orbis.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.World
{
    /// <summary>
    /// Author: Bram Kelder
    /// </summary>
    class Scene
    {
        public Map WorldMap { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public int Seed { get; private set; }
        public BiomeCollection BiomeCollection { get; private set; }

        public XMLModel.WorldSettings Settings { get; private set; }

        public Scene(int seed, XMLModel.WorldSettings settings, BiomeCollection biomeCollection)
        {
            Seed = seed;
            Settings = settings;
            BiomeCollection = biomeCollection;
        }
    }
}
