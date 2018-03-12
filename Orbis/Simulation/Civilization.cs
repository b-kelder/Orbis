using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.World;

namespace Orbis.Simulation
{
    class Civilization
    {
        /// <summary>
        /// The name of the civ
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// If the civ has died
        /// </summary>
        public bool Dead { get; set; }
        /// <summary>
        /// The cells owned by this civ
        /// </summary>
        public List<Cell> Territory { get; set; }

        public List<Cell> neighbours { get; set; }

        /// <summary>
        /// The devensive strength of this civ
        /// </summary>
        public int DefenceModifier { get; set; }
        /// <summary>
        /// The offensive strength of this civ
        /// </summary>
        public int OffenceModifier { get; set; }
        /// <summary>
        /// The current progress to of technology
        /// </summary>
        public double TechnologicalProgress { get; set; }

        /// <summary>
        /// The current population of the civ
        /// </summary>
        public int Population { get; set; }
        /// <summary>
        /// The current supply of food for the civ
        /// </summary>
        public double Food { get; set; }
        /// <summary>
        /// The current wealth of the civ
        /// </summary>
        public double Wealth { get; set; }
        /// <summary>
        /// The current amount of resources of the civ
        /// </summary>
        public double Resources { get; set; }

        private int totalHousing;

        //TODO: Add personalisations

        public Civilization()
        {
            Territory = new List<Cell>();
            neighbours = new List<Cell>();
        }

        /// <summary>
        /// Determane what action to perform next
        /// </summary>
        /// <returns></returns>
        public Action DetermineAction()
        {
            if (Population > totalHousing)
            {
                return ExpandTerritory;
            }

            // Return the do nothing action
            return null;
        }

        /// <summary>
        /// Expand territory to the best neighbor cell
        /// </summary>
        /// <returns></returns>
        public void ExpandTerritory()
        {
            foreach (Cell neighbour in neighbours.AsParallel())
            {
                if (ClaimCell(neighbour))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Add a cell to this civs territory
        /// </summary>
        /// <param name="cell">Cell to add</param>
        /// <returns>True if succesfull</returns>
        public bool ClaimCell(Cell cell)
        {
            if (cell.Owner != null || cell.IsWater)
            {
                return false;
            }
            cell.Owner = this;
            Territory.Add(cell);

            foreach (Cell c in cell.Neighbours)
            {
                if (c.Owner != this)
                {
                    neighbours.Add(c);
                }
            }

            neighbours.Remove(cell);

            totalHousing += cell.Housing;

            return true;
        }
    }
}
