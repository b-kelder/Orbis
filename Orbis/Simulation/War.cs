using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int Duration { get; private set; }
        public Civilization Attacker { get; set; }
        public Civilization Defender { get; set; }

        private Random _random;
        private int _upperBound;
        private int _lowerBound;
        private Scene _scene;
        

        /// <summary>
        ///  Start a new war between two civs.
        /// </summary>
        /// <param name="attacker">The initiator of the war.</param>
        /// <param name="defender">The defending civ.</param>
        public War(Scene scene, Civilization attacker, Civilization defender)
        {
            _random = new Random(scene.Seed);
            _upperBound = 100;
            _lowerBound = 0;
            _scene = scene;
            Attacker = attacker;
            Defender = defender;
            Duration = 1;

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
        public Boolean Battle()
        {
            bool warEnded = false;

            // TODO: Add other modifiers to this calculation
            int battleResult = (int)Math.Floor(_random.Next(1, 21)
                + (0.4 * Attacker.Population + 10 * Attacker.Wars.Count)
                - (0.4 * Defender.Population + 10 * Defender.Wars.Count));

            System.Diagnostics.Debug.WriteLine("Battle result for war between {0} and {1}: {2}.",
                Attacker.Name,
                Defender.Name,
                battleResult);

            Attacker.Population -= (int)Math.Floor((double)battleResult * (5 * Duration));
            Defender.Population -= (int)Math.Floor((double)battleResult * (5 * Duration));

            if(battleResult > _upperBound - 5 * Duration)
            {
                warEnded = true;

                // TODO: implement GetWarResultCells.

                System.Diagnostics.Debug.WriteLine("The war between {0} ({1}) and {2} ({3}) has been won by {0}.",
                    Attacker.Name,
                    Attacker.Population,
                    Defender.Name,
                    Defender.Population);
            }
            else if(battleResult < _lowerBound + 5 * Duration)
            {
                warEnded = true;

                System.Diagnostics.Debug.WriteLine("The war between {0} ({1}) and {2} ({3}) has been won by {2}.",
                    Attacker.Name,
                    Attacker.Population,
                    Defender.Name,
                    Defender.Population);
            }

            if (warEnded)
            {
                Attacker.BorderCivs.Remove(Defender);
                Attacker.CivOpinions.Remove(Defender);
                Attacker.Wars.Remove(this);

                Defender.BorderCivs.Remove(Attacker);
                Defender.CivOpinions.Remove(Attacker);
                Defender.Wars.Remove(this);
            }

            Duration++;
            return warEnded;
        }

        /// <summary>
        ///     Determine the cells that will be transferred by this war.
        /// </summary>
        /// <param name="victory">
        ///     Whether the attacker or the defender won the war.
        /// </param>
        /// <returns>
        ///     The bordering cells between the two civs.
        /// </returns>
        public Cell[] GetWarResultCells(bool victory)
        {
            if (victory)
            {
                return Defender.Territory.ToArray();
            }
            else
            {
                return Attacker.Territory.ToArray();
            }
            // TODO: Get cells to transfer.
        }
    }
}
