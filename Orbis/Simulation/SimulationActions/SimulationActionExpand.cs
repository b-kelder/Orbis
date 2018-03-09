using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Simulation.SimulationActions
{
    class SimulationActionExpand : ISimuationAction
    {
        Civilization Civ;

        public SimulationActionExpand(Civilization civ)
        {
            Civ = civ;
        }

        public void PerformAction()
        {
            //Debug.WriteLine(Civ.Name + " wants to expand.");

            Civ.ExpandTerritory();
        }
    }
}
