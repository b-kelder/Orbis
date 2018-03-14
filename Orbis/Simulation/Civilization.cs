﻿using System;
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
        public bool IsAlive { get; set; }
        /// <summary>
        /// The cells owned by this civ
        /// </summary>
        public List<Cell> Territory { get; set; }
        /// <summary>
        /// All neighbour cells of the civs territory
        /// </summary>
        public HashSet<Cell> Neighbours { get; set; }
        /// <summary>
        /// The total population of the civ
        /// </summary>
        public int Population { get; set; }

        public double BaseExpand = 2;
        public double BaseExploit = 1;
        public double BaseExplore = 1;
        public double BaseExterminate = 1;

        private double housingNeed = 1;
        private double foodNeed = 1;
        private double resourceNeed = 1;
        private double wealthNeed = 1;
        private double warNeed = -1;

        public Civilization()
        {
            IsAlive = true;
            Territory = new List<Cell>();
            Neighbours = new HashSet<Cell>();
        }

        /// <summary>
        /// Determane what action to perform next
        /// </summary>
        /// <returns></returns>
        public SimulationAction DetermineAction()
        {
            SimulationAction simulationAction = new SimulationAction(this, Simulation4XAction.DONOTHING, null);

            double expand = 1, exploit = 1, explore = 1, exterminate = 1;

            expand *= BaseExpand;
            exploit *= BaseExploit;
            explore *= BaseExplore;
            exterminate *= BaseExterminate;

            if (expand > exploit && expand > explore && expand > exterminate)
            {
                Cell cell = Neighbours.First();

                int cellCount = Neighbours.Count;
                foreach (Cell c in Neighbours)
                {
                    if (CalculateCellValue(c) > CalculateCellValue(cell))
                    {
                        cell = c;
                    }
                }

                simulationAction.Action = Simulation4XAction.EXPAND;
                simulationAction.Params = new object[] { cell };
            }
            else if (exploit > expand && exploit > explore && exploit > exterminate)
            {
                simulationAction.Action = Simulation4XAction.EXPLOIT;
                simulationAction.Params = null;
            }
            else if (explore > expand && explore > exploit && explore > exterminate)
            {
                simulationAction.Action = Simulation4XAction.EXPLORE;
                simulationAction.Params = null;
            }
            else if (exterminate > expand && exterminate > exploit && exterminate > explore)
            {
                simulationAction.Action = Simulation4XAction.EXTERMINATE;
                simulationAction.Params = null;
            }

            return simulationAction;
        }

        public double CalculateCellValue(Cell cell)
        {
            // Calculate value based on needs.
            double val = (cell.MaxHousing / 1000 * housingNeed) + (cell.FoodMod * foodNeed) + (cell.ResourceMod * resourceNeed) + (cell.WealthMod * wealthNeed);

            // Add value for each neighbour cell.
            int cellCount = cell.Neighbours.Count;
            for (int i = 0; i < cellCount; i++)
            {
                if (cell.Neighbours[i].Owner == this)
                {
                    val += 1;
                }
                else if (cell.Neighbours[i].Owner != null)
                {
                    val += warNeed;
                }
            }

            return val;
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

            foreach (Cell c in cell.Neighbours)
            {
                if (c.Owner != this)
                {
                    Neighbours.Add(c);
                }
            }

            Neighbours.Remove(cell);

            return true;
        }
    }
}
