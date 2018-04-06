using System;
using System.Collections.Generic;
using System.Linq;
using Orbis.Events;
using Orbis.Events.Exporters;
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
        private static int BATTLE_VICTORY_THRESHOLD = 3;    // The battle result value above which a battle is won by the attacker.
        private static int BATTLE_DEFEAT_THRESHOLD = 0;     // The battle result value below which a battle is lost by the attacker.
        private static float DURATION_MOD = 0.3F;           // The value by which the duration is multiplied for the battle result.
        private static int WAR_END_THRESHOLD = 20;          // The minimum end score value required for a war to end.

        // Logger messages.
        private static string WAR_START = "{0} has declared war on {1}.";                               // Displayed when a war is declared.
        private static string BATTLE_WON = "{0} has defeated {1} in battle.";                           // Displayed when a battle is won.
        private static string BATTLE_STALEMATE = "A battle between {0} and {1} ended in a stalemate.";  // Displayed when a battle ends in a tie.
        private static string WAR_END = "The war between {0} and {1} has ended.";                       // Displayed when a war ends.
        #endregion

        public Civilization Attacker { get; set; }
        public Civilization Defender { get; set; }

        private Random _random;
        private Scene _scene;
        private int _battleBalance;
        private int _duration;
        private Logger _logger;
        
        // amount of lost battles/ duration of the war / random shit

        /// <summary>
        ///  Start a new war between two civs.
        /// </summary>
        /// <param name="attacker">The initiator of the war.</param>
        /// <param name="defender">The defending civ.</param>
        public War(Scene scene, Civilization attacker, Civilization defender)
        {
            _random = new Random(scene.Seed);
            _scene = scene;
            _battleBalance = 0;
            _duration = 1;
            Attacker = attacker;
            Defender = defender;

            Attacker.StartWar(this);
            Defender.StartWar(this);

            _logger = Logger.GetInstance();
            _logger.AddLog(string.Format(WAR_START, Attacker.Name, Defender.Name));
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
                int battleResult = (int)Math.Floor(_random.Next(-2, 3) * (DURATION_MOD * _duration) +
                    + ((double)Attacker.Population / Defender.Population)
                    + ((double)Attacker.TotalWealth / Defender.TotalWealth)
                    + ( Defender.WarCount - Attacker.WarCount));

                System.Diagnostics.Debug.WriteLine("Battle result for battle between " + Attacker.Name + " and " + Defender.Name + ": " + battleResult);

                if (battleResult > BATTLE_VICTORY_THRESHOLD)
                {
                    result.Winner = Attacker;
                    result.OccupiedTerritory = GetOccupiedTerritory(Attacker, Defender);

                    _logger.AddLog(string.Format(BATTLE_WON, Attacker.Name, Defender.Name), "war");
                }
                else if (battleResult < BATTLE_DEFEAT_THRESHOLD)
                {
                    result.Winner = Defender;
                    result.OccupiedTerritory = GetOccupiedTerritory(Defender, Attacker);

                    _logger.AddLog(string.Format(BATTLE_WON, Defender.Name, Attacker.Name), "war");
                }
                else
                {
                    _logger.AddLog(string.Format(BATTLE_STALEMATE, Attacker.Name, Defender.Name), "war");
                }

                int endScore = _random.Next(1, 6) - _battleBalance + _duration;

                warEnded = (endScore > WAR_END_THRESHOLD);
            }

            if (warEnded)
            {
                _logger.AddLog(String.Format(WAR_END, Attacker.Name, Defender.Name), "war");

                Attacker.EndWar(this);
                Defender.EndWar(this);
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
