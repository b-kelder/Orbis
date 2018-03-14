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

namespace Orbis.Simulation
{
    class Simulator
    {
        public int CurrentTick { get; set; }
        public Scene Scene { get; set; }

        private ConcurrentQueue<SimulationAction> actionQueue;
        private List<Cell> cellsChanged;
        private int maxTick;
        private int civCount;

        private Random rand;

        private Task simulationTask;
        private List<Task> taskList;

        public Simulator(Scene scene, int simulationLength)
        {
            Scene = scene;
            maxTick = simulationLength;
            civCount = scene.Civilizations.Count;

            rand = new Random(scene.Seed);

            actionQueue = new ConcurrentQueue<SimulationAction>();
            taskList = new List<Task>();
            cellsChanged = new List<Cell>();
        }

        public Cell[] GetChangedCells()
        {
            return cellsChanged.ToArray();
        }

        public void Update()
        {
            if (CurrentTick >= maxTick)
            {
                return;
            }

            if (simulationTask == null || simulationTask.IsCompleted)
            {
                simulationTask = Task.Run(()=> 
                {
                    Tick();
                });
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
                    int cellCount = civ.Territory.Count;
                    for (int j = 0; j < cellCount; j++)
                    {
                        Cell cell = civ.Territory[j];

                        if (cell.population <= 0)
                        {
                            continue;
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

                    }
                }
            }

            cellsChanged = changed;
        }
    }
}
