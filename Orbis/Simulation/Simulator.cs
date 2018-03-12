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
        /// 
        /// </summary>
        private Task task;

        /// <summary>
        /// The current tick in the simulation
        /// </summary>
        public int Tick { get; set; }

        public int SimLength { get; set; }

        /// <summary>
        /// Queue of all by civs chosen actions.
        /// </summary>
        private Queue<Action> actionQueue;

        /// <summary>
        /// Create a simulator object
        /// </summary>
        public Simulator()
        {
            actionQueue = new Queue<Action>();
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
            SimLength = length;

            actionQueue = new Queue<Action>();

            Tick = 0;
        }

        /// <summary>
        /// Update function called every update
        /// </summary>
        public void Update()
        {
            if (Tick >= SimLength)
            {
                return;
            }
            if (task == null || task.IsCompleted)
            {
                task = Task.Run(()=> StartTick());
            }
        }

        private void StartTick()
        {
            Tick++;

            foreach (Civilization civ in Scene.Civilizations)
            {
                // Check if the civ is still alive.
                if (civ.Dead)
                {
                    continue;
                }

                // Let the civ decide an action
                actionQueue.Enqueue(civ.DetermineAction());

                // Go through each cell and claim resources for this tick
                foreach (Cell cell in civ.Territory.AsParallel())
                {
                    civ.Food += Dice.Roll(5, 5) * cell.FoodMod;
                    civ.Wealth += Dice.Roll(5, 5) * cell.WealthMod;
                    civ.Resources += Dice.Roll(5, 5) * cell.ResourceMod;
                }

                // Calculate birth
                int births = Dice.Roll(3, civ.Population / 5);

                // Calculate deaths
                int PeopleWithNoFood = (int)Math.Ceiling(civ.Population - civ.Food);
                int deaths = Dice.Roll(10, civ.Population / 5) + PeopleWithNoFood;

                // Grow population based on birth and deaths
                civ.Population += births - deaths;
            }  

            while (actionQueue.Count > 0)
            {
                Action simuationAction = actionQueue.Dequeue();
                if (simuationAction != null)
                {
                    simuationAction.Invoke();
                }
            }
        }
    }
}
