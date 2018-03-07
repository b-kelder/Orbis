using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis
{
    static class Dice
    {
        static Random random = new Random(192836);

        /// <summary>
        /// Roll any amount of dice with any amount of sides and get the total result.
        /// </summary>
        /// <param name="amount">The amount of dice to roll</param>
        /// <returns></returns>
        public static int Roll(int sides, int amount)
        {
            int result = 0;
            for (int i = 0; i < amount; i++)
            {
                result += random.Next(1, sides + 1);
            }

            return result;
        }
    }
}
