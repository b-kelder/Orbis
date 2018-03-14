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
        public int CurrentTick { get; set; }
        public Scene Scene { get; set; }
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

        public Simulator(Scene scene, int simulationLength)
        {
            Scene = scene;
            maxTick = simulationLength;
            civCount = scene.Civilizations.Count;
            TickLengthInSeconds = 0;

            rand = new Random(scene.Seed);

            actionQueue = new ConcurrentQueue<SimulationAction>();
            taskList = new List<Task>();
            cellsChanged = new ConcurrentQueue<Cell[]>();
            ongoingWars = new List<War>();
        }

        public Cell[] GetChangedCells()
        {
            Cell[] cells;
            cellsChanged.TryDequeue(out cells);
            return cells;
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentTick >= maxTick)
            {
                return;
            }

            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if ((simulationTask == null || simulationTask.IsCompleted) && elapsedTime > TickLengthInSeconds)
            {
                simulationTask = Task.Run(()=> 
                {
                    Tick();
                });

                elapsedTime = 0;
            }
        }

        public void Tick()
        {
            CurrentTick++;

            taskList.Clear();

            for (int i = 0; i < civCount; i++)
            {
                Civilization civ = Scene.Civilizations[i];
                if (!civ.IsAlive)
                {
                    continue;
                }

                actionQueue.Enqueue(civ.DetermineAction());

                taskList.Add(Task.Run(() =>
                {
                    bool hasLand = false;

                    foreach (var cell in civ.Territory)
                    {
                        if (cell.population <= 0)
                        {
                            // Remove cell from territory
                            continue;
                        }

                        if (!cell.IsWater)
                        {
                            hasLand = true;
                        }

                        int roll = rand.Next(5, 25);
                        cell.food += roll * 5 * cell.FoodMod;
                        cell.resources += roll * 5 * cell.ResourceMod;
                        cell.wealth += roll * 5 * cell.WealthMod;

                        int peopleWithNoFood = (int)Math.Ceiling(cell.population - cell.food);
                        int birth = 3 * rand.Next(0, cell.population / 5);
                        int death = rand.Next(0, cell.population / 5) + peopleWithNoFood;

                        cell.population += birth - death;

                        civ.Population += birth - death;
                        civ.TotalResource += roll * 5 * cell.ResourceMod;
                        civ.TotalWealth += roll * 5 * cell.WealthMod;
                    }

                    if (!hasLand)
                    {
                        civ.IsAlive = false;
                    }
                }));
            }

            Task.WaitAll(taskList.ToArray());

            List<Cell> changed = new List<Cell>();

            while (actionQueue.Count > 0)
            {
                SimulationAction action;
                if (actionQueue.TryDequeue(out action))
                {
                    if (action.Action == Simulation4XAction.EXPAND)
                    {
                        Cell cell = (Cell)action.Params[0];
                        if (action.Civilization.ClaimCell(cell))
                        {
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
                        Civilization civ = (Civilization)action.Params[0];


                    }
                }
            }

            cellsChanged.Enqueue(changed.ToArray());
        }
    }
}
