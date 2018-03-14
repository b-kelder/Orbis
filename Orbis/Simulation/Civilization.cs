﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.World;

/// <summary>
/// Author: Bram Kelder, Wouter Brookhuis
/// </summary>
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
        /// Is currently at war
        /// </summary>
        public bool AtWar { get; set; }
        /// <summary>
        /// The cells owned by this civ
        /// </summary>
        public HashSet<Cell> Territory { get; set; }
        /// <summary>
        /// All neighbour cells of the civs territory
        /// </summary>
        public HashSet<Cell> Neighbours { get; set; }
        /// <summary>
        /// The total population of the civ
        /// </summary>
        public int Population { get; set; }

        public int TotalHousing { get; set; }
        public double TotalWealth { get; set; }
        public double TotalResource { get; set; }

        public double BaseExpand = 3;
        public double BaseExploit = 1;
        public double BaseExplore = 1;
        public double BaseExterminate = 0;

        private double housingNeed = 1;
        private double foodNeed = 1;
        private double resourceNeed = 1;
        private double wealthNeed = 1;

        public Civilization()
        {
            IsAlive = true;
            Territory = new HashSet<Cell>();
            Neighbours = new HashSet<Cell>();
        }

        /// <summary>
        /// Determane what action to perform next
        /// </summary>
        /// <returns></returns>
        public SimulationAction DetermineAction()
        {
            SimulationAction action = new SimulationAction(this, Simulation4XAction.DONOTHING, null);

            if (AtWar)
            {
                return action;
            }

            double expand = 1, exploit = 1, explore = 1, exterminate = 1;

            expand *= BaseExpand + (Population > 0 ? ((Population - (double)TotalHousing) / Population) : 0);
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

                action.Action = Simulation4XAction.EXPAND;
                action.Params = new object[] { cell };
            }
            else if (exploit > expand && exploit > explore && exploit > exterminate)
            {
                action.Action = Simulation4XAction.EXPLOIT;
                action.Params = null;
            }
            else if (explore > expand && explore > exploit && explore > exterminate)
            {
                action.Action = Simulation4XAction.EXPLORE;
                action.Params = null;
            }
            else if (exterminate > expand && exterminate > exploit && exterminate > explore)
            {
                Civilization civilization = null;

                action.Action = Simulation4XAction.EXTERMINATE;
                action.Params = new object[] { civilization };
            }

            return action;
        }

        public double CalculateCellValue(Cell cell)
        {
            // Calculate value based on needs.
            double val = (cell.MaxHousing / 1000 * housingNeed) + (cell.FoodMod * foodNeed) + (cell.ResourceMod * resourceNeed) + (cell.WealthMod * wealthNeed);

            if (cell.Owner == null)
            {
                val += 2.5;
            }
            else if (cell.Owner != null)
            {
                val += -4 + BaseExterminate;
            }

            // Add value for each neighbour cell.
            int cellCount = cell.Neighbours.Count;
            for (int i = 0; i < cellCount; i++)
            {
                if (cell.Neighbours[i].Owner == this)
                {
                    val += 2.5;
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
            if(cell.Owner != null)
            {
                // Recalculate neighbours, TODO: do war
                cell.Owner.Territory.Remove(cell);
                // Get neighbouring territory
                HashSet<Cell> neighbouringTerritory = new HashSet<Cell>();
                foreach(var n1 in cell.Neighbours)
                {
                    foreach(var n2 in n1.Neighbours)
                    {
                        if(n2.Owner == cell.Owner && n2 != cell)
                        {
                            neighbouringTerritory.Add(n2);
                        }
                    }
                    if(n1.Owner == cell.Owner)
                    {
                        neighbouringTerritory.Add(n1);
                    }
                    cell.Owner.Territory.Remove(n1);
                }
                // Update based on territory
                foreach(var territory in neighbouringTerritory)
                {
                    foreach (Cell c in territory.Neighbours)
                    {
                        if (c.Owner != this && c != cell)
                        {
                            Neighbours.Add(c);
                        }
                    }
                }
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
            TotalHousing += cell.MaxHousing;

            return true;
        }
    }
}
