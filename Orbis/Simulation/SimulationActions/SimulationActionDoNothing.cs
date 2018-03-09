using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Simulation.SimulationActions
{
    class SimulationActionDoNothing : ISimuationAction
    {
        public void PerformAction()
        {
            // Leave empty as the civ decided to perform no action.
        }
    }
}
