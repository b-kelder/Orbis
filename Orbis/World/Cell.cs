using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.Engine;
using Orbis.Simulation;

/// <summary>
/// Author: Bram Kelder
/// </summary>
namespace Orbis.World
{
    public class Cell
    {
        #region constants
        const float HARD_CAP = 10000;
        #endregion

        /// <summary>
        /// List of neighbour cells
        /// </summary>
        public List<Cell> Neighbours { get; set; }
        /// <summary>
        /// The current owner of the cell
        /// </summary>
        public Civilization Owner { get; set; }
        /// <summary>
        /// Food modifier for yields
        /// </summary>
        public double FoodMod { get; set; }
        /// <summary>
        /// Resource modifier for yields
        /// </summary>
        public double ResourceMod { get; set; }
        /// <summary>
        /// Wealth modifier for yields
        /// </summary>
        public double WealthMod { get; set; }
        /// <summary>
        /// Max population that can be housed in this cell
        /// </summary>
        public int MaxHousing { get; set; }
        /// <summary>
        /// Terrain elevation level
        /// </summary>
        public double Elevation { get; set; }
        /// <summary>
        /// Indicates if this entire cell is water
        /// </summary>
        public bool IsWater { get; set; }
        /// <summary>
        /// Indicates if this cell has a river.
        /// </summary>
        public bool HasRiver { get; set; }
        /// <summary>
        /// Indicates how wet this cell is in equivalents of mms of rain per year.
        /// </summary>
        public double Wetness { get; set; }
        /// <summary>
        /// Coordinates of the cell on the world map
        /// </summary>
        public Point Coordinates { get; set; }
        /// <summary>
        /// The biome of this cell
        /// </summary>
        public Biome Biome { get; set; }
        /// <summary>
        /// Temperature of this cell in degrees Celcius
        /// </summary>
        public float Temperature { get; set; }


        public int population;
        public double food;
        public double resources;
        public double wealth;
        public SettlementSize settlementSize;

        public Cell(Point coordinates)
        {
            Coordinates = coordinates;
        }

        /// <summary>
        ///     Simulate this cell.
        /// </summary>
        /// <param name="rand">The random to use for simulation.</param>
        /// <returns>Whether the owner lost the cell.</returns>
        public bool Simulate(Random rand)
        {
            if (population <= 0 && !IsWater)
            {
                return true;
            }

            // Roll a dice for the food, wealth and resource harvest
            int roll = rand.Next(5, 25);

            // Calculate food, wealth and resources based on cell modifiers
            food = MathHelper.Clamp((float)food + roll * 100 * (float)FoodMod, 0, HARD_CAP);
            double newResources = MathHelper.Clamp((float)resources + roll * 5 * (float)ResourceMod, 0, HARD_CAP);
            double newWealth = MathHelper.Clamp((float)wealth + roll * 5 * (float)WealthMod, 0, HARD_CAP);

            // Calculate the amount of people without food
            int peopleWithNoFood = (int)Math.Ceiling(population - food);

            // Calculate births based of the sie of the cells Population
            int birth = 3 * rand.Next(0, population / 5);

            // Calculate deaths based on cells Population and the amount of people without food
            int death = rand.Next(0, population / 5) + peopleWithNoFood;

            // Eat food
            food = MathHelper.Clamp((float)food - population, 0, HARD_CAP);

            // Clamp the max Population to the max housing of the cell
            int newPopulation = MathHelper.Clamp(population + birth - death, 0, MaxHousing);

            // Get the differences between the old and new values so we can modify the owner.
            int populationDiff = newPopulation - population;
            double resourcesDiff = newResources - resources;
            double wealthDiff = newWealth - wealth;

            // Write the new values to the fields.
            population = newPopulation;
            resources = newResources;
            wealth = newWealth;

            // Update the owner's values.
            Owner.TotalResource += resourcesDiff;
            Owner.TotalWealth += wealthDiff;
            Owner.Population += populationDiff;

            // The cell didn't lose its owner.
            return false;
        }
    }
}