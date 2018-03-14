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
        public Map WorldMap { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public int Seed { get; set; }

        public Scene(int seed)
        {
            Seed = seed;
        }
    }
}
