using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.World;

/// <summary>
/// Author: Bram Kelder, Wouter Brookhuis, Kaj van der Veen
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
        public bool AtWar
        {
            get
            {
                return (Wars.Count != 0);
            }
        }

        /// <summary>
        /// The current wars for this Civ.
        /// </summary>
        public List<War> Wars;
        /// <summary>
        /// The cells owned by this civ
        /// </summary>
        public HashSet<Cell> Territory { get; set; }
        /// <summary>
        /// All neighbour cells of the civs territory
        /// </summary>
        public HashSet<Cell> Neighbours { get; set; }
        /// <summary>
        ///     All civs this civ borders.
        /// </summary>
        public HashSet<Civilization> BorderCivs { get; set; }
        /// <summary>
        ///     The opinion this civ has of neighbouring civs.
        /// </summary>
        public Dictionary<Civilization, int> CivOpinions { get; set; }
        /// <summary>
        /// The total population of the civ
        /// </summary>
        public int Population { get; set; }

        public int TotalHousing { get; set; }
        public double TotalWealth { get; set; }
        public double TotalResource { get; set; }

        public double BaseExpand = 1;
        public double BaseExploit = 1;
        public double BaseExplore = 1;
        public double BaseExterminate = 1;

        private double housingNeed = 1;
        private double foodNeed = 1;
        private double resourceNeed = 1;
        private double wealthNeed = 1;

        public Civilization()
        {
            IsAlive = true;
            Territory = new HashSet<Cell>();
            Neighbours = new HashSet<Cell>();
            Wars = new List<War>();
            BorderCivs = new HashSet<Civilization>();
            CivOpinions = new Dictionary<Civilization, int>();
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

            expand *= BaseExpand + (Population / (double)TotalHousing);
            exploit *= BaseExploit;
            explore *= BaseExplore;

            // TODO: make picking targets use logic instead of "The first one I hate (which is the first guy I border) will do".
            Civilization iHate = BorderCivs.FirstOrDefault(c => CivOpinions[c] <= -100);
            exterminate *= BaseExterminate + (iHate != null ? ((Math.Abs(CivOpinions[iHate]) - 100) / 20) : 0);

            if (expand > exploit && expand > explore && expand > exterminate)
            {
                if (Neighbours.Count <= 0)
                {
                    LoseCell(Territory.First());
                    return action;
                }

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
                if (iHate != null)
                {
                    action.Action = Simulation4XAction.EXTERMINATE;
                    action.Params = new object[] { iHate };
                }
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
                val *= -10 + BaseExterminate;
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

        public bool LoseCell(Cell cell)
        {
            if (cell.Owner != this)
            {
                return false;
            }

            if (!Territory.Remove(cell))
            {
                return false;
            }

            cell.Owner = null;

            Neighbours.Add(cell);

            List<Cell> a = new List<Cell>();
            foreach (Cell c in cell.Neighbours)
            {
                if (c.Owner == this)
                {
                    foreach (Cell cc in c.Neighbours)
                    {
                        if (cc.Owner != this)
                        {
                            Neighbours.Add(cc);
                        }
                    }
                }
                else
                {
                    a.Add(c);
                }
            }

            foreach (Cell c in a)
            {
                Neighbours.Remove(c);
            }

            return true;
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
                cell.Owner.LoseCell(cell);
            }

            cell.Owner = this;
            Territory.Add(cell);

            foreach (Cell c in cell.Neighbours)
            {
                if (c.Owner != this)
                {
                    Neighbours.Add(c);

                    if (c.Owner != null)
                    {
                        if (BorderCivs.Add(c.Owner))
                        {
                            CivOpinions.Add(c.Owner, -20);
                        }
                        else
                        {
                            CivOpinions[c.Owner] -= 20;
                        }
                    }
                }
            }

            Neighbours.Remove(cell);
            TotalHousing += cell.MaxHousing;

            cell.population = 100;

            return true;
        }
    }
}
