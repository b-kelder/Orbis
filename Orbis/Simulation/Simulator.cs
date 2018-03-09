using Microsoft.Xna.Framework;
using Orbis.Engine;
using Orbis.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.Simulation
{
    class Simulator
    {
        /// <summary>
        /// The scene to simulate for
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// The current tick in the simulation
        /// </summary>
        public int Tick { get; set; }

        /// <summary>
        /// Queue of all by civs chosen actions.
        /// </summary>
        private Queue<ISimuationAction> actionQueue;

        /// <summary>
        /// Create a simulator object
        /// </summary>
        public Simulator()
        {
            actionQueue = new Queue<ISimuationAction>();
        }

        /// <summary>
        /// Create a simulator object that simulates for any amout of ticks
        /// </summary>
        /// <param name="scene">The scene to simulate for</param>
        /// <param name="length">Amount of ticks to simulate</param>
        public Simulator(Scene scene, int length)
        {
            if (scene.Civilizations.Count <= 0)
            {
                throw new ArgumentException("No civs to simulate in this scene");
            }

            Scene = scene;
        }

        /// <summary>
        /// Update function called every update
        /// </summary>
        public void Update()
        {
            foreach (Civilization civ in Scene.Civilizations)
            {
                // Check if the civ is still alive.
                if (civ.Dead)
                {
                    continue;
                }

                // Go through each cell and claim resources for this tick
                foreach (Cell cell in civ.Territory)
                {
                    civ.Food += Dice.Roll(10, 10) * cell.FoodMod;
                    civ.Wealth += Dice.Roll(10, 10) * cell.WealthMod;
                    civ.Resources += Dice.Roll(10, 10) * cell.ResourceMod;
                }

                // Calculate birth
                int births = Dice.Roll(4, civ.Population / 5);

                // Calculate deaths
                int PeopleWithNoFood = (int)Math.Ceiling(civ.Population - civ.Food);
                int deaths = Dice.Roll(4, civ.Population / 5) + PeopleWithNoFood;

                // Grow population based on birth and deaths
                civ.Population += births - deaths;

                if (civ.Population <= 0)
                {
                    civ.Dead = true;
                    Debug.WriteLine(civ.Name + " died.");
                }
                else
                {
                    //if (civ.Population > totalHousing)
                    {

                    }
                }

                Debug.WriteLine(civ.Name + " Population: " + civ.Population + " Size: " + civ.Territory.Count);
            }

            Tick++;
        }
    }
}
