using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.Simulation.SimulationActions;
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

        //TODO: Add personalisations

        public Civilization()
        {
            Territory = new List<Cell>();
        }

        /// <summary>
        /// Determane what action to perform next
        /// </summary>
        /// <returns></returns>
        public ISimuationAction DetermineAction()
        {
            int totalHousing = 0;
            foreach (Cell cell in Territory)
            {
                totalHousing += cell.Housing;
            }

            if (Population > totalHousing)
            {
                return new SimulationActionExpand(this);
            }

            // Return the do nothing action
            return new SimulationActionDoNothing();
        }

        /// <summary>
        /// Expand territory to the best neighbor cell
        /// </summary>
        /// <returns></returns>
        public Cell ExpandTerritory()
        {
            foreach (Cell cell in Territory)
            {
                foreach (Cell neighbour in cell.Neighbours)
                {
                    if (ClaimCell(neighbour))
                    {
                        return neighbour;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Add a cell to this civs territory
        /// </summary>
        /// <param name="cell">Cell to add</param>
        /// <returns>True if succesfull</returns>
        public bool ClaimCell(Cell cell)
        {
            if (cell.Owner != null)
            {
                return false;
            }
            cell.Owner = this;
            Territory.Add(cell);

            return true;
        }
    }
}
