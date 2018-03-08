using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.World
{
    class Civilization
    {
        /// <summary>
        /// The name of the civ
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The cells owned by this civ
        /// </summary>
        public List<Cell> Territory { get; set; }
        /// <summary>
        /// The devensive strength of this civ
        /// </summary>
        public int DefenceModifier { get; set; }
        /// <summary>
        /// The offensive strength of this civ
        /// </summary>
        public int OffenceModifier { get; set; }

        /// <summary>
        /// The current population of the civ
        /// </summary>
        public int Population { get; set; }
        /// <summary>
        /// The current progress to of technology
        /// </summary>
        public int TechnologyProgress { get; set; }
        /// <summary>
        /// The current wealth of the civ
        /// </summary>
        public int Wealth { get; set; }

        //TODO: Add personalisations

        public Civilization()
        {
            Territory = new List<Cell>();
        }
    }
}
