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
        public Cell[,] WorldMap { get; set; }
        public List<Civilization> Civilizations { get; set; }

        public Scene()
        {

        }
    }
}
