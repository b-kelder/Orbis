using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Bram Kelder
/// </summary>
namespace Orbis.Simulation
{
    public enum CivDecision
    {
        // Expand territory
        EXPAND,
        // Declare war
        EXTERMINATE,
        // Don't do anything
        DONOTHING
    }

    public class SimulationAction
    {
        public Civilization Civilization { get; set; }
        public CivDecision Action { get; set; }
        public object[] Params { get; set; }

        public SimulationAction(Civilization civilization, CivDecision action, object[] @params)
        {
            Civilization = civilization;
            Action = action;
            Params = @params;
        }
    }
}
