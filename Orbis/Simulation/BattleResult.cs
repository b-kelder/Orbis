using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbis.World;

namespace Orbis.Simulation
{
    /// <summary>
    ///     Represents the result of a single battle during a war.
    /// </summary>
    public class BattleResult
    {
        /// <summary>
        ///     The civilisation that won the battle.
        /// </summary>
        public Civilization Winner { get; set; }

        /// <summary>
        ///     The territory that was occupied in the battle.
        /// </summary>
        public Cell[] OccupiedTerritory { get; set; }

        /// <summary>
        ///     Create a new BattleResult with default values.
        /// </summary>
        public BattleResult()
        {
            OccupiedTerritory = new Cell[0];
        }
    }
}
