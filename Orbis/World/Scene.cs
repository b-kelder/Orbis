using Orbis.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.World
{
    class Scene
    {
        //public Cell[,] WorldMap { get; set; }
        public Map WorldMap { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public int Seed { get; private set; }

        public XMLModel.WorldSettings Settings { get; private set; }

        public Scene(int seed, XMLModel.WorldSettings settings)
        {
            Seed = seed;
            Settings = settings;
        }
    }
}
