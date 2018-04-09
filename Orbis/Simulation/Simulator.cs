using Microsoft.Xna.Framework;
using Orbis.World;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Author: Bram Kelder
/// </summary>
namespace Orbis.Simulation
{
    /// <summary>
    ///     Handles simulation of the world.
    /// </summary>
    public class Simulator
    {
        /// <summary>
        /// Current tick of the simulation.
        /// </summary>
        public int CurrentTick { get; set; }

        /// <summary>
        /// The date of the simulation.
        /// </summary>
        public static DateTime Date { get; set; }

        /// <summary>
        /// The scene to simulate.
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// The minimum length of a tick in seconds.
        /// </summary>
        public double TickLengthInSeconds { get; set; }

        /// <summary>
        /// The maximum tick for the simulation, being the last one.
        /// </summary>
        public int MaxTick { get; set; }

        // Private Variables
        private ConcurrentQueue<Cell[]> cellsChanged;
        private ConcurrentQueue<SimulationAction> actionQueue;
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
            MaxTick = simulationLength;
            civCount = scene.Civilizations.Count;
            TickLengthInSeconds = 0;

            Date = new DateTime(2166, 1, 1);

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

        /// <summary>
        /// Toggle between paused and running state
        /// </summary>
        public void TogglePause()
        {
            pause = !pause;
        }

        /// <summary>
        /// Check if the simualtion is paused
        /// </summary>
        /// <returns>
        ///     True if paused
        /// </returns>
        public bool IsPaused()
        {
            return pause;
        }

        /// <summary>
        /// Simulate a single tick
        /// </summary>
        public void SimulateOneTick()
        {
            if (IsPaused() && CurrentTick < MaxTick)
            {
                Tick();
            }
        }

        /// <summary>
        /// Get an array of cells that changed owner
        /// </summary>
        /// <returns>
        ///     The changed cells from the last tick
        /// </returns>
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
        /// <param name="gameTime">
        ///     GameTime object
        /// </param>
        public void Update(GameTime gameTime)
        {
            // Check if the max ticks has been reached
            if (MaxTick > 0 && CurrentTick >= MaxTick || pause)
            {
                pause = true;
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
            IncreaseTick();

            // Create a list to store the changed cells
            List<Cell> changed = new List<Cell>();

            // Clear the list of tasks the civs want to perform
            // Should be empty, but just in case
            taskList.Clear();

            // Simulate all civilizations in the scene
            changed.AddRange(SimulateCivilization());

            // Go through all cells that need to lose owner
            foreach (KeyValuePair<Cell, Civilization> cellKeyValue in removeOwner)
            {
                // Set the Population to 0;
                cellKeyValue.Key.population = 0;
                // Remove the cell from the civs territory
                cellKeyValue.Value.LoseCell(cellKeyValue.Key);
                // Add the cells to the changed cells list
                changed.Add(cellKeyValue.Key);
            }
            removeOwner.Clear();

            // Perform all the actions chosen by the civs
            changed.AddRange(PerformCivilizationActions());

            // Simulate all ongoning wars
            changed.AddRange(SimulateWars());

            // Enqueue the changed cells
            cellsChanged.Enqueue(changed.ToArray());
        }

        /// <summary>
        /// Increment the tick and date
        /// </summary>
        private void IncreaseTick()
        {
            // Increment the current tick
            CurrentTick++;
            // Increment the date
            Date = Date.AddMonths(1);
        }

        /// <summary>
        /// Simulate all civilizations
        /// All civilizations run in a seperate task, the function returns if all tasks are completed
        /// </summary>
        /// <returns>
        ///     A list of cells changed during the simulation
        /// </returns>
        private Cell[] SimulateCivilization()
        {
            ConcurrentBag<Cell> changed = new ConcurrentBag<Cell>();

            // Loop through all civilizations in the scene
            for (int i = 0; i < civCount; i++)
            {
                // Get the civ
                Civilization civ = Scene.Civilizations[i];

                bool hasTerritory = civ.Territory.Count != 0;

                // Dead civs or civs without land tiles that do still have territory have their territory removed.
                if ((!civ.IsAlive || !civ.HasLand) && hasTerritory)
                {
                    // Use a parallel loop to increase performance for larger civs.
                    Parallel.ForEach(civ.Territory, cell => {
                        removeOwner.TryAdd(cell, civ);
                    });
                }

                if (!hasTerritory)
                {
                    // skip civs without territory.
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
                    Cell[] cells = CivilizationTask(civ);

                    int cellCount = cells.Length;
                    for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
                    {
                        Cell cell = cells[cellIndex];
                        changed.Add(cell);
                    }
                }));
            }

            // Wait for all tasks to complete
            // (All civs)
            Task.WaitAll(taskList.ToArray());

            return changed.ToArray();
        }

        /// <summary>
        /// The task to run for a civilization
        /// </summary>
        /// <param name="civilization">
        ///     The civilization to run the task for
        /// </param>
        /// <returns>
        ///     A list of cells changed during the simulation
        /// </returns>
        private Cell[] CivilizationTask(Civilization civilization)
        {
            Cell[] changedCells = SimulateCells(civilization);

            return changedCells;
        }

        /// <summary>
        /// Simulate all cells of a civ
        /// </summary>
        /// <param name="civilization">
        ///     The civilization to simulate all cells for
        /// </param>
        /// <returns>
        ///     A list of cells changed during the simulation
        /// </returns>
        private Cell[] SimulateCells(Civilization civilization)
        {
            List<Cell> changed = new List<Cell>();

            // Use a parralel loop to speed up territory simulation of large civs.
            Parallel.ForEach(civilization.Territory, cell => {
                if (cell.Simulate(rand))
                {
                    removeOwner.TryAdd(cell, cell.Owner);
                }
                else if(WentOverPopulationThreshold(cell))
                {
                    changed.Add(cell);
                }
            });

            return changed.ToArray();
        }

        /// <summary>
        /// Check population threshold for updating tile decorations
        /// </summary>
        /// <param name="cell">
        ///     The cell to check
        /// </param>
        /// <param name="newPopulation">
        ///     The new amount of population
        /// </param>
        /// <returns>
        ///     True if over a new threshold
        /// </returns>
        private bool WentOverPopulationThreshold(Cell cell)
        {
            bool crossedThreshold = false;
            // Check the current size and whether it crossed a threshold, updating the size appropriately.
            if (cell.settlementSize == SettlementSize.Tiny)
            {
                crossedThreshold = cell.population > Scene.DecorationSettings.SmallPopulationThreshold;
                cell.settlementSize = SettlementSize.Small;
            }
            else if (cell.settlementSize == SettlementSize.Small)
            {
                crossedThreshold = cell.population > Scene.DecorationSettings.MediumPopulationThreshold;
                cell.settlementSize = SettlementSize.Medium;
            }
            else if (cell.settlementSize == SettlementSize.Medium)
            {
                crossedThreshold = cell.population > Scene.DecorationSettings.LargePopulationThreshold;
                cell.settlementSize = SettlementSize.Large;
            }

            return crossedThreshold;
        }

        /// <summary>
        /// Perform actions chosen by the civs
        /// </summary>
        /// <returns>
        ///     A list of cells changed during the simulation
        /// </returns>
        private Cell[] PerformCivilizationActions()
        {
            List<Cell> changed = new List<Cell>();

            // Loop through all the actions from the civ
            while (actionQueue.Count > 0)
            {
                // Try to dequeue the next action
                if (actionQueue.TryDequeue(out SimulationAction action))
                {
                    if (action.Action == CivDecision.EXPAND)
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
                    else if (action.Action == CivDecision.EXTERMINATE)
                    {
                        // Get the civ to declare war on
                        Civilization defender = (Civilization)action.Params[0];

                        // Ensure that two countries aren't at war twice at the same time.
                        if (ongoingWars.Find(w => (w.Defender == action.Civilization && w.Attacker == defender)) != null) continue;

                        // Create a new war
                        War war = new War(action.Civilization, defender, Scene.Seed);
                        // Add to the ongoing war list
                        ongoingWars.Add(war);
                    }
                }
            }

            return changed.ToArray();
        }

        /// <summary>
        ///     Simulate all ongoing wars
        /// </summary>
        /// 
        /// /// <returns>
        ///     A list of cells changed during the simulation
        /// </returns>
        private Cell[] SimulateWars()
        {
            List<Cell> changed = new List<Cell>();

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

            return changed.ToArray();
        }
    }
}