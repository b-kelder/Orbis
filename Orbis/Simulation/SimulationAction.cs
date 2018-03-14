using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Simulation
{
    enum Simulation4XAction
    {
        // Explore
        EXPLORE,
        // Expand territory
        EXPAND,
        // Double resource earnings and increase research for this tick
        EXPLOIT,
        // Declare or fight war
        EXTERMINATE,
        // Don't do anything
        DONOTHING
    }

    class SimulationAction
    {
        public Civilization Civilization { get; set; }
        public Simulation4XAction Action { get; set; }
        public object[] Params { get; set; }

        public SimulationAction(Civilization civilization, Simulation4XAction action, object[] @params)
        {
            Civilization = civilization;
            Action = action;
            Params = @params;
        }
    }
}
