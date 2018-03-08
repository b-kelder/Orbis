using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.World
{
    class Biome
    {
        public string Name { get; set; }
        /// <summary>
        /// Modifier for the amount of food within the biome used as a basis for cell modifiers
        /// </summary>
        public int FoodModifier { get; set; }
        /// <summary>
        /// Modifier for the amount of recources in the biome  used as a basis for cell modifiers
        /// </summary>
        public int ResourceModifier { get; set; }
        /// <summary>
        /// Modifier for the amount of usable space for the population  used as a basis for cell modifiers
        /// </summary>
        public int PopulationModifier { get; set; }


        public Biome()
        {

        }
    }
}
