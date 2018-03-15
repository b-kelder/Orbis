using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Bram Kelder
/// </summary>
namespace Orbis.World
{
    class Biome
    {
        public string Name { get; set; }
        /// <summary>
        /// Modifier for the amount of food within the biome used as a basis for cell modifiers
        /// </summary>
        public double FoodModifier { get; set; }
        /// <summary>
        /// Modifier for the amount of recources in the biome  used as a basis for cell modifiers
        /// </summary>
        public double ResourceModifier { get; set; }
        /// <summary>
        /// Modifier for the amount of usable space for the population  used as a basis for cell modifiers
        /// </summary>
        public double PopulationModifier { get; set; }


        public Biome(XMLModel.Biome data)
        {
            this.ResourceModifier = data.ResourceModifier;
            this.PopulationModifier = data.PopulationModifier;
            this.FoodModifier = data.FoodModifier;
            this.Name = data.Name;
        }
    }
}
