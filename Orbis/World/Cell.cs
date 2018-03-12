using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.Engine;
using Orbis.Simulation;

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
        public int Housing { get; set; }
        /// <summary>
        /// Terrain elevation level
        /// </summary>
        public double Elevation { get; set; }
        /// <summary>
        /// Indicates if this entire cell is water
        /// </summary>
        public bool IsWater { get; set; }

        public Point Coordinates { get; set; }

        public Cell(Point coordinates)
        {
            Coordinates = coordinates;
        }
    }
}