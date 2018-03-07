using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.Engine;

namespace Orbis.World
{
    class Cell
    {
        /// <summary>
        /// List of neighbour cells
        /// </summary>
        public List<Cell> Neighbours { get; set; }
        /// <summary>
        /// The current owner of the cell
        /// </summary>
        public Civilization Owner { get; set; }

        /// <summary>
        /// The biome that this cell is based on
        /// </summary>
        public Biome Biome { get; set; }
        /// <summary>
        /// Modifier for the amount of food within the cell
        /// </summary>
        public int FoodValue { get; set; }
        /// <summary>
        /// Modifier for the amount of recources in the cell
        /// </summary>
        public int ResourceValue { get; set; }
        /// <summary>
        /// Modifier for the amount of usable space for the population
        /// </summary>
        public int PopulationValue { get; set; }

        public Cell()
        {
            
        }

        /// <summary>
        /// Calculate modifiers based on dice rolls and the biome
        /// </summary>
        public void CalculateModifiers()
        {
            // Generate basis value off a D20 roll
            FoodValue = Dice.Roll(20, 1);
            ResourceValue = Dice.Roll(20, 1);
            PopulationValue = Dice.Roll(20, 1);

            // Add biome modifiers
            FoodValue += Biome.FoodModifier;
            ResourceValue += Biome.ResourceModifier;
            PopulationValue += Biome.PopulationModifier;

            // Add modifiers based on terrain
            //TODO: Modifiers based on proximity of water, mountains.
        }
    }
}
