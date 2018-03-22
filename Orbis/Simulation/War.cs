using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Orbis.World;

namespace Orbis.Simulation
{
    /// <summary>
    ///     Represents a war between two civilizations.
    /// </summary>
    /// <author>
    ///     Kaj van der Veen
    /// </author>
    class War
    {
        public Civilization Attacker { get; set; }
        public Civilization Defender { get; set; }

        private Random _random;
        private int _upperBound;
        private int _lowerBound;
        private Scene _scene;
        private int _battleBalance;
        private int _duration;
        
        // amount of lost battles/ duration of the war / random shit

        /// <summary>
        ///  Start a new war between two civs.
        /// </summary>
        /// <param name="attacker">The initiator of the war.</param>
        /// <param name="defender">The defending civ.</param>
        public War(Scene scene, Civilization attacker, Civilization defender)
        {
            _random = new Random(scene.Seed);
            _upperBound = 3;
            _lowerBound = 0;
            _scene = scene;
            _battleBalance = 0;
            _duration = 1;
            Attacker = attacker;
            Defender = defender;

            Attacker.Wars.Add(this);
            Defender.Wars.Add(this);

            System.Diagnostics.Debug.WriteLine("{0} has declared war on {1}."
                , Attacker.Name
                , Defender.Name);
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
                int battleResult = (int)Math.Floor(_random.Next(-2, 3) * (0.3 * _duration) +
                    + ((double)Attacker.Population / Defender.Population)
                    + ((double)Attacker.TotalWealth / Defender.TotalWealth)
                    + ( Defender.Wars.Count - Attacker.Wars.Count));


                //+ (0.4 * Attacker.Population + 10 * Attacker.Wars.Count)
                //- (0.4 * Defender.Population + 10 * Defender.Wars.Count));

                System.Diagnostics.Debug.WriteLine("Battle result for battle between {0} and {1}: {2}.",
                    Attacker.Name,
                    Defender.Name,
                    battleResult);

                if (battleResult > _upperBound)
                {
                    result.Winner = Attacker;
                    result.OccupiedTerritory = GetOccupiedTerritory(Attacker, Defender);

                    System.Diagnostics.Debug.WriteLine("{0} ({1}) has won a battle against {2} ({3}).",
                        Attacker.Name,
                        Attacker.Population,
                        Defender.Name,
                        Defender.Population);
                }
                else if (battleResult < _lowerBound)
                {
                    result.Winner = Defender;
                    result.OccupiedTerritory = GetOccupiedTerritory(Defender, Attacker);

                    System.Diagnostics.Debug.WriteLine("{2} ({3}) has won a battle against {0} ({1}).",
                        Attacker.Name,
                        Attacker.Population,
                        Defender.Name,
                        Defender.Population);
                }

                int endScore = _random.Next(1, 6) - _battleBalance + _duration;

                warEnded = (endScore > 20);
            }

            if (warEnded)
            {
                System.Diagnostics.Debug.WriteLine("The war between {0} ({1}) and {2} ({3}) has ended. (duration: {4}).",
                        Attacker.Name,
                        Attacker.Population,
                        Defender.Name,
                        Defender.Population,
                        _duration);

                Attacker.Wars.Remove(this);
                Defender.Wars.Remove(this);

                // Add real war cooldown for civs.
                Attacker.BorderCivs.Remove(Defender);
                Attacker.CivOpinions.Remove(Defender);

                Defender.BorderCivs.Remove(Attacker);
                Defender.CivOpinions.Remove(Attacker);
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
