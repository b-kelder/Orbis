using Microsoft.Xna.Framework;
using Orbis.Engine;
using Orbis.World;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Bram Kelder
/// </summary>
namespace Orbis.Simulation
{
    class Simulator
    {
        /// <summary>
        /// Current tick of the simulation
        /// </summary>
        public int CurrentTick { get; set; }
        /// <summary>
        /// The scene to simulate
        /// </summary>
        public Scene Scene { get; set; }
        /// <summary>
        /// The minimum length of a tick in seconds
        /// </summary>
        public double TickLengthInSeconds { get; set; }

        private ConcurrentQueue<Cell[]> cellsChanged;
        private ConcurrentQueue<SimulationAction> actionQueue;
        private int maxTick;
        private int civCount;
        private double elapsedTime = 0;

        private Random rand;

        private Task simulationTask;
        private List<Task> taskList;

        private List<War> ongoingWars;
        private ConcurrentDictionary<Cell, Civilization> removeOwner;

        private bool pause;

        /// <summary>
        /// Create a simulator
        /// </summary>
        /// <param name="scene">The scene to simulate</param>
        /// <param name="simulationLength">How many ticks to simulate</param>
        public Simulator(Scene scene, int simulationLength)
        {
            // Set vars
            Scene = scene;
            maxTick = simulationLength;
            civCount = scene.Civilizations.Count;
            TickLengthInSeconds = 0;

            // Create a random based on the scene's seed
            rand = new Random(scene.Seed);
            pause = true;
            // Create lists and queues
            actionQueue = new ConcurrentQueue<SimulationAction>();
            taskList = new List<Task>();
            cellsChanged = new ConcurrentQueue<Cell[]>();
            ongoingWars = new List<War>();
            removeOwner = new ConcurrentDictionary<Cell, Civilization>();
        }

        public void TogglePause()
        {
            pause = !pause;
        }

        public bool IsPaused()
        {
            return pause;
        }

        /// <summary>
        /// Get an array of cells that changed owner
        /// </summary>
        /// <returns>The changed cells from the last tick</returns>
        public Cell[] GetChangedCells()
        {
            // Get the array of cells from the changed cells queue
            cellsChanged.TryDequeue(out Cell[] cells);
            // Return the cells
            return cells;
        }

        /// <summary>
        /// Update fucntion that needs to be called every update that the simulation should run
        /// </summary>
        /// <param name="gameTime">GameTime object</param>
        public void Update(GameTime gameTime)
        {
            // Check if the max ticks has been reached
            if (CurrentTick >= maxTick || pause)
            {
                return;
            }

            // Increase elapsed time since start of tick
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            // If the minimum time has elapsed and no tick is running
            if ((simulationTask == null || simulationTask.IsCompleted) && elapsedTime > TickLengthInSeconds)
            {
                // Start a new tick in a new task
                simulationTask = Task.Run(()=> 
                {
                    Tick();
                });

                // Reset elapsed task
                elapsedTime = 0;
            }
        }

        /// <summary>
        /// Simulate a single tick
        /// </summary>
        public void Tick()
        {
            // Increase the current tick
            CurrentTick++;

            // Clear the list of tasks the civs want to perform
            // Should be empty, but just in case
            taskList.Clear();

            // Create a new queue to track changed cells this tick
            // Needs to be concurrent because of multiple tasks
            ConcurrentQueue<Cell> changedCells = new ConcurrentQueue<Cell>();

            // Loop through all civilizations in the scene
            for (int i = 0; i < civCount; i++)
            {
                // Get the civ
                Civilization civ = Scene.Civilizations[i];
                // Skip the civ if its dead
                if (!civ.IsAlive)
                {
                    continue;
                }

                // Let the civ decide an action
                SimulationAction action = civ.DetermineAction();
                if (action != null)
                {
                    actionQueue.Enqueue(civ.DetermineAction());
                }

                // Create a task to simulate the progress for a civ this tick
                taskList.Add(Task.Run(() =>
                {
                    // Keep track of some vars
                    bool hasLand = false;
                    int population = 0;
                    double wealth = 0;
                    double resources = 0;

                    // Loop through all cells owned by this civ
                    foreach (var cell in civ.Territory)
                    {
                        // If population is negative or zero
                        if (cell.population <= 0)
                        {
                            // Add the cell to a list to remove its owner
                            removeOwner.TryAdd(cell, civ);
                            
                            // Skip the simulation
                            continue;
                        }

                        // If the cell is not water, set the has land flag
                        if (!cell.IsWater)
                        {
                            hasLand = true;
                        }

                        // Roll a dice for the food, wealth and resource harvest
                        int roll = rand.Next(5, 25);
                        // Calculate food, wealth and resources based on cell modifiers
                        cell.food += roll * 5 * cell.FoodMod;
                        cell.resources += roll * 5 * cell.ResourceMod;
                        cell.wealth += roll * 5 * cell.WealthMod;

                        // Calculate the amount of people without food
                        int peopleWithNoFood = (int)Math.Ceiling(cell.population - cell.food);
                        // Calculate births based of the sie of the cells population
                        int birth = 3 * rand.Next(0, cell.population / 5);
                        // Calculate deaths based on cells population and the amount of people without food
                        int death = rand.Next(0, cell.population / 5) + peopleWithNoFood;

                        ///
                        ///
                        /// TEST PURE MAGIC NEEDS TO BE LOOKED AT MAYBE??
                        ///
                        ///
                        if(WentOverPopulationThreshold(cell, MathHelper.Clamp(cell.population + birth - death, 0, cell.MaxHousing)))
                        {
                            changedCells.Enqueue(cell);
                        }

                        // Clamp the max population to the max housing of the cell
                        cell.population = MathHelper.Clamp(cell.population + birth - death, 0, cell.MaxHousing);

                        // Add gains to local trackers to update civ data at end of tick
                        population += cell.population;
                        wealth += cell.wealth;
                        resources += cell.resources;
                    }

                    // If a civ does not have land or population
                    if (!hasLand || population <= 0)
                    {
                        // set civ to dead
                        civ.IsAlive = false;
                        // Set the population to 0 to prevent negative numbers
                        civ.Population = 0;
                        // Go through all cells
                        foreach (Cell cell in civ.Territory)
                        {
                            // Add the cells to the remove owner list
                            removeOwner.TryAdd(cell, civ);
                        }
                    }

                    // Update the total values of the civ
                    civ.TotalResource = resources;
                    civ.TotalWealth = wealth;
                    civ.Population = population;

                }));
            }

            // Wait for all tasks to complete
            // (All civs)
            Task.WaitAll(taskList.ToArray());
            // Create a list to store the changed cells
            List<Cell> changed = new List<Cell>();

            // Add the cells changed last tick...
            changed.AddRange(changedCells);

            // Go through all cells that need to lose owner
            foreach (KeyValuePair<Cell, Civilization> cellKeyValue in removeOwner)
            {
                // Set the population to 0;
                cellKeyValue.Key.population = 0;
                // Remove the cell from the civs territory
                cellKeyValue.Value.LoseCell(cellKeyValue.Key);
                // Add the cells to the changed cells list
                changed.Add(cellKeyValue.Key);
            }

            // Loop through all the actions from the civ
            while (actionQueue.Count > 0)
            {
                // Try to dequeue the next action
                if (actionQueue.TryDequeue(out SimulationAction action))
                {
                    if (action.Action == Simulation4XAction.EXPAND)
                    {
                        // Get the cell to claim
                        Cell cell = (Cell)action.Params[0];
                        // Claim the cell
                        if (action.Civilization.ClaimCell(cell))
                        {
                            // On succes, add to changed list
                            changed.Add(cell);
                        }
                    }
                    else if (action.Action == Simulation4XAction.EXPLOIT)
                    {

                    }
                    else if (action.Action == Simulation4XAction.EXPLORE)
                    {

                    }
                    else if (action.Action == Simulation4XAction.EXTERMINATE)
                    {
                        // Get the civ to declare war on
                        Civilization defender = (Civilization)action.Params[0];
                        // Create a new war
                        War war = new War(Scene, action.Civilization, defender);
                        // Add to the ongoing war list
                        ongoingWars.Add(war);
                    }
                }
            }

            // War simulations :D
            int warCount = ongoingWars.Count;
            // Get the result of the current battle in each ongoing war.
            for (int warIndex = 0; warIndex < warCount; warIndex++)
            {
                // Get the war object
                War war = ongoingWars[warIndex];
                // Get the result from the war
                bool warEnded = war.Battle(out BattleResult battleResult);

                if (battleResult.Winner != null)
                {
                    // Go through all cells
                    foreach (Cell cell in battleResult.OccupiedTerritory)
                    {
                        battleResult.Winner.ClaimCell(cell);

                        changed.Add(cell);
                    }
                }

                // If the war is over
                if (warEnded)
                {
                    // Wars that have come to an end are removed from the ongoing wars list.
                    warIndex--;
                    warCount--;
                    ongoingWars.Remove(war);
                }
            }

            // Enqueue the changed cells
            cellsChanged.Enqueue(changed.ToArray());
        }

        private bool WentOverPopulationThreshold(Cell cell, int newPopulation)
        {
            return (cell.population < Scene.DecorationSettings.SmallPopulationThreshold &&
                newPopulation >= Scene.DecorationSettings.SmallPopulationThreshold) ||
                (cell.population < Scene.DecorationSettings.MediumPopulationThreshold &&
                newPopulation >= Scene.DecorationSettings.MediumPopulationThreshold) ||
                (cell.population < Scene.DecorationSettings.LargePopulationThreshold &&
                newPopulation >= Scene.DecorationSettings.LargePopulationThreshold);
        }
    }
}