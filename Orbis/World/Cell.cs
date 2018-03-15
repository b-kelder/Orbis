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

        public double food;
        public double resources;
        public double wealth;
        public int population;

        public Cell(Point coordinates)
        {
            Coordinates = coordinates;
        }
    }
}