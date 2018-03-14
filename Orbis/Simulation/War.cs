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

        private Random _random;
        private int _upperBound;
        private int _lowerBound;
        private Scene _scene;
        private Civilization _attacker;
        private Civilization _defender;

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
            _attacker = attacker;
            _defender = defender;
            Duration = 1;

            System.Diagnostics.Debug.WriteLine("{0} has declared war on {1}."
                , _attacker
                , _defender);
        }

        /// <summary>
        ///     Decides the outcome of a battle tick between two civs.
        /// </summary>
        /// <returns></returns>
        public Boolean Battle()
        {
            bool warEnded = false;

            // TODO: Add amount of wars and other modifiers to this calculation
            int battleResult = (int)Math.Floor(_random.Next(1, 21)
                + (0.4 * _attacker.Population + 10 * 1)
                - (0.4 * _defender.Population + 10 * 1));

            System.Diagnostics.Debug.WriteLine("Battle result for war between {0} and {1}: {2}.",
                _attacker,
                _defender,
                battleResult);

            _attacker.Population -= (int)Math.Floor((double)battleResult * (5 * Duration));
            _defender.Population -= (int)Math.Floor((double)battleResult * (5 * Duration));

            if(battleResult > _upperBound - 5 * Duration)
            {
                warEnded = true;

                // TODO: implement GetWarResultCells.
                Cell[] toTransfer = GetWarResultCells(true);
                foreach (Cell cell in toTransfer)
                {
                    //_attacker.ClaimCell(cell);
                }

                System.Diagnostics.Debug.WriteLine("The war between {0} and {1} has been won by {0}.",
                    _attacker,
                    _defender);
            }
            else if(battleResult < _lowerBound + 5 * Duration)
            {
                warEnded = true;

                // TODO: implement GetWarResultCells.
                Cell[] toTransfer = GetWarResultCells(true);
                foreach (Cell cell in toTransfer)
                {
                    //_attacker.ClaimCell(cell);
                }

                System.Diagnostics.Debug.WriteLine("The war between {0} and {1} has been won by {1}.",
                    _attacker,
                    _defender);
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
        private Cell[] GetWarResultCells(bool victory)
        {
            // TODO: Get cells to transfer.
            return new Cell[1];
        }
    }
}
