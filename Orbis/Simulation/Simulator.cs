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
        private ConcurrentDictionary<Cell, Civilization> cc;

        private bool pause = false;

        public Simulator(Scene scene, int simulationLength)
        {
            Scene = scene;
            maxTick = simulationLength;
            civCount = scene.Civilizations.Count;
            TickLengthInSeconds = 0.1;

            rand = new Random(scene.Seed);

            actionQueue = new ConcurrentQueue<SimulationAction>();
            taskList = new List<Task>();
            cellsChanged = new ConcurrentQueue<Cell[]>();
            ongoingWars = new List<War>();
            cc = new ConcurrentDictionary<Cell, Civilization>();
        }

        public void Reset()
        {

        }

        public bool IsPaused()
        {
            return pause;
        }

        public void Pause()
        {
            pause = true;
        }

        public void Resume()
        {
            pause = false;
        }

        public Cell[] GetChangedCells()
        {
            cellsChanged.TryDequeue(out Cell[] cells);
            return cells;
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentTick >= maxTick || pause)
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
                    int population = 0;

                    foreach (var cell in civ.Territory)
                    {
                        if (cell.population <= 0)
                        {
                            cc.TryAdd(cell, civ);
                            
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

                        population += cell.population;
                        civ.TotalResource += roll * 5 * cell.ResourceMod;
                        civ.TotalWealth += roll * 5 * cell.WealthMod;
                    }

                    if (!hasLand || population <= 0)
                    {
                        civ.IsAlive = false;
                        civ.SetPopulation(0);
                        foreach (Cell cell in civ.Territory)
                        {
                            cc.TryAdd(cell, civ);
                        }
                    }

                    civ.SetPopulation(population);
                }));
            }

            Task.WaitAll(taskList.ToArray());

            List<Cell> changed = new List<Cell>();

            foreach (KeyValuePair<Cell, Civilization> ccc in cc)
            {
                ccc.Key.population = 0;
                ccc.Value.LoseCell(ccc.Key);
                changed.Add(ccc.Key);
            }

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
                        Civilization defender = (Civilization)action.Params[0];
                        War war = new War(Scene, action.Civilization, defender);
                        ongoingWars.Add(war);
                    }
                }
            }

            int warCount = ongoingWars.Count;
            HashSet<War> finishedWars = new HashSet<War>();
            // Get the result of the current battle in each ongoing war.
            for (int warIndex = 0; warIndex < warCount; warIndex++)
            {
                War war = ongoingWars[warIndex];
                bool warResult = war.Battle();
                if (warResult)
                {
                    Cell[] toTransfer = war.GetWarResultCells(warResult);

                    foreach (Cell cell in toTransfer)
                    {
                        if (warResult)
                        {
                            war.Attacker.ClaimCell(cell);
                        }
                        else
                        {
                            war.Defender.ClaimCell(cell);
                        }
                        
                        changed.Add(cell);
                    }

                    finishedWars.Add(war);
                }
            }

            // Wars that have come to an end are removed from the ongoing wars list.
            foreach (War war in finishedWars)
            {
                ongoingWars.Remove(war);
            }

            cellsChanged.Enqueue(changed.ToArray());
        }
    }
}
