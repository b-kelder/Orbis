using System;
using System.Collections.Generic;
using System.Linq;
using Orbis.Events;

using Orbis.World;

namespace Orbis.Simulation
{
    /// <summary>
    ///     Represents a war between two civilizations.
    /// </summary>
    /// 
    /// <author> Kaj van der Veen</author>
    public class War
    {
        #region Constants
        // Calculation values.
        private const int BATTLE_VICTORY_THRESHOLD = 3;    // The battle result value above which a battle is won by the attacker.
        private const int BATTLE_DEFEAT_THRESHOLD = 12;     // The battle result value below which a battle is lost by the attacker.
        private const int WAR_END_THRESHOLD = 20; // The minimum end score value required for a war to end.

        // Logger messages.
        private const string WAR_START = "{0} has declared war on {1}.";                               // Displayed when a war is declared.
        private const string BATTLE_WON = "{0} has defeated {1} in battle.";                           // Displayed when a battle is won.
        private const string BATTLE_STALEMATE = "A battle between {0} and {1} ended in a stalemate.";  // Displayed when a battle ends in a tie.
        private const string WAR_END = "The war between {0} and {1} has ended.";                       // Displayed when a war ends.
        #endregion

        /// <summary>
        ///     The attacking civ in this war.
        /// </summary>
        public Civilization Attacker { get; set; }

        /// <summary>
        ///     The defending civ in this war.
        /// </summary>
        public Civilization Defender { get; set; }

        // amount of lost battles/ duration of the war / random things
        private Random _random;
        private int _battleBalance;
        private int _duration;
        private Logger _logger;

        /// <summary>
        ///     Start a new war between two civs.
        /// </summary>
        /// <param name="attacker">The initiator of the war.</param>
        /// <param name="defender">The defending civ.</param>
        /// <param name="seed">The seed used for random outcomes.</param>
        public War( Civilization attacker, Civilization defender, int seed)
        {
            _random = new Random(seed);
            _battleBalance = 0;
            _duration = 1;
            Attacker = attacker;
            Defender = defender;

            // Notify the participants that the war has properly started.
            Attacker.StartWar(this);
            Defender.StartWar(this);

            // Create the logger and log the start of the war.
            _logger = Logger.GetInstance();
            _logger.AddWithGameTime(string.Format(WAR_START, Attacker.Name, Defender.Name), Simulator.Date, "war");
        }

        /// <summary>
        ///     Decides the outcome of a battle tick between two civs.
        /// </summary>
        /// <returns></returns>
        public bool Battle(out BattleResult result)
        {
            bool warEnded = false;

            result = new BattleResult();

            if (!Attacker.IsAlive || !Defender.IsAlive)
            {
                warEnded = true;
            }
            else
            {
                // Result strongly favours the bigger civ, however there is a chance of the smaller civ pushing back.
                int battleResult = (int)Math.Floor( _random.Next(1, 4)
                    + ((double)Attacker.Population / Defender.Population)
                    + (Attacker.TotalWealth / Defender.TotalWealth)
                    + (Defender.WarCount - Attacker.WarCount));

                if (battleResult > BATTLE_VICTORY_THRESHOLD)
                {
                    result.Winner = Attacker;
                    result.OccupiedTerritory = GetOccupiedTerritory(Attacker, Defender);
                    _battleBalance++;

                    _logger.AddWithGameTime(string.Format(BATTLE_WON, Attacker.Name, Defender.Name), Simulator.Date, "war");
                }
                else if (battleResult < BATTLE_DEFEAT_THRESHOLD)
                {
                    result.Winner = Defender;
                    result.OccupiedTerritory = GetOccupiedTerritory(Defender, Attacker);
                    _battleBalance--;

                    _logger.AddWithGameTime(string.Format(BATTLE_WON, Defender.Name, Attacker.Name), Simulator.Date, "war");
                }
                else
                {
                    _logger.AddWithGameTime(string.Format(BATTLE_STALEMATE, Attacker.Name, Defender.Name), Simulator.Date, "war");
                }

                int endScore = _random.Next(1, 6) - _battleBalance + _duration;

                warEnded = (endScore > WAR_END_THRESHOLD || endScore < -WAR_END_THRESHOLD);
            }

            if (warEnded)
            {
                // Notify the participants that the war has ended.
                Attacker.EndWar(this);
                Defender.EndWar(this);

                _logger.AddWithGameTime(string.Format(WAR_END, Attacker.Name, Defender.Name), Simulator.Date, "war");
            }

            _duration++;

            return warEnded;
        }

        /// <summary>
        ///     Determine the cells that will be transferred by a battle.
        /// </summary>
        /// 
        /// <param name="winner">
        ///     The winner of the battle.
        /// </param>
        /// <param name="loser">
        ///     The loser of the battle.
        /// </param>
        /// 
        /// <returns>
        ///     The bordering cells between the two civs.
        /// </returns>
        private Cell[] GetOccupiedTerritory(Civilization winner, Civilization loser)
        {
            HashSet<Cell> cells = new HashSet<Cell>();
            foreach (Cell cell in winner.Neighbours)
            {
                if (cell.Owner == loser)
                {
                    cells.Add(cell);
                }
            }

            return cells.ToArray();
        }
    }
}
