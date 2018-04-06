using Microsoft.Xna.Framework;
using Orbis.Simulation;
using Orbis.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Orbis.World
{
    /// <summary>
    /// Author: Bram Kelder, Wouter Brookhuis, Kaj v.d. Veen, Auke Muller
    /// </summary>
    class WorldGenerator
    {
        /// <summary>
        /// Random object
        /// </summary>
        private Random random;
        private Scene scene;
        private XMLModel.Civilization[] civSettings;

        /// <summary>
        /// World generator constructor
        /// </summary>
        /// <param name="seed">A seed to base generation on</param>
        public WorldGenerator(Scene scene, XMLModel.Civilization[] civSettings)
        {
            this.scene          = scene;
            this.civSettings    = civSettings;

            // Create a random object based on a seed
            random = new Random(scene.Seed);
        }

        public void GenerateCivs(int count)
        {
            var availableCells = new List<Point>();
            for (int x = -scene.WorldMap.Radius; x <= scene.WorldMap.Radius; x++)
            {
                for (int y = -scene.WorldMap.Radius; y <= scene.WorldMap.Radius; y++)
                {
                    var cellPoint = new Point(x, y);
                    availableCells.Add(cellPoint);
                }
            }
            
            scene.Civilizations = new List<Civilization>();
            for(int i = 0; i < count; i++)
            {
                // Create a civ with all base values
                Civilization civ = new Civilization();
                if (i >= civSettings.Length)
                {
                    // Random names if more civs requested then there are names for
                    XMLModel.Civilization civ1 = civSettings[random.Next(0, civSettings.Length)];
                    XMLModel.Civilization civ2 = civSettings[random.Next(0, civSettings.Length)];
                    civ.Name = civ1 + " the " + civ2.name;
                    civ.Color = new Color(civ1.Color.R, civ2.Color.G, civ2.Color.B);
                }
                else
                {
                    civ.Name    = civSettings[i].name;
                    civ.Color   = new Color(civSettings[i].Color.R, civSettings[i].Color.G, civSettings[i].Color.B);
                }


                // Select a random starting cell for the civ
                // Loop until no cell is available or until break
                while (scene.Civilizations.Count < scene.WorldMap.CellCount && availableCells.Count > 0)
                {
                    // TODO: Make random function that always gives tile within radius?
                    // Get a random available cell from the range of available cells and remove it from the list of available cells.
                    var nextCell = availableCells[random.Next(0, availableCells.Count)];
                    availableCells.Remove(nextCell);

                    // Check if the cell has an owner
                    var cell = scene.WorldMap.GetCell(nextCell.X, nextCell.Y);

                    if (cell != null)
                    {
                        // No atlantis shenanigans
                        if(cell.IsWater)
                        {
                            continue;
                        }

                        cell.Owner = civ;
                        civ.Territory.Add(cell);

                        foreach (Cell c in cell.Neighbours)
                        {
                            if (c.Owner != civ && !c.IsWater)
                            {
                                civ.Neighbours.Add(c);
                            }
                        }

                        cell.population = 1;
                        civ.TotalHousing += cell.MaxHousing;

                        break;
                    }
                }

                if (civ.Territory.Count > 0)
                {
                    // Add the civ to the world
                    scene.Civilizations.Add(civ);
                }
                else
                {
                    break;
                }
            }
        }

        public void GenerateWorld(int radius)
        {
            double statLowestPoint = double.MaxValue;
            double statHighestMountain = double.MinValue;
            int statSeaTiles = 0;

            Perlin perlin = new Perlin(5);
            float boundsX = TopographyHelper.HexToWorld(new Point(radius, 0)).X;
            float boundsY = TopographyHelper.HexToWorld(new Point(0, radius)).Y;
            var perlinZ = random.NextDouble();

            scene.WorldMap = new Map(radius);

            for(int p = -radius; p <= radius; p++)
            {
                for(int q = -radius; q <= radius; q++)
                {
                    var currentPoint = new Point(p, q);
                    var cell = scene.WorldMap.GetCell(p, q);
                    if(cell == null)
                    {
                        continue;
                    }

                    // TODO: Remove Magic Numbers, put them in WorldSettings

                    // Set cell neighbors
                    cell.Neighbours = scene.WorldMap.GetNeighbours(currentPoint);

                    // Set cell height
                    var worldPoint = TopographyHelper.HexToWorld(currentPoint);
                    var perlinPoint = (worldPoint + new Vector2(boundsX, boundsY)) * 0.01f;
                    var elevation = perlin.OctavePerlin(perlinPoint.X, perlinPoint.Y, perlinZ, 4, 0.7);
                    // Flat valleys
                    elevation = Math.Pow(elevation, scene.Settings.ElevationPower);

                    // Actually multiply to max height
                    elevation *= scene.Settings.ElevationMultiplier;

                    // Give cells closer to the edge of the map a negative bias. This means the entire map will be surrounded by oceans.
                    var distanceFromEdge = (radius - TopographyHelper.Distance(new Point(0, 0), currentPoint)) / radius;
                    if(distanceFromEdge < 0 || distanceFromEdge > 1)
                    {
                        throw new Exception("Value out of range, check math");
                    }
                    cell.Elevation = elevation * Math.Pow(distanceFromEdge, scene.Settings.EdgeDistancePower);

                    // Update stats
                    if(cell.Elevation > statHighestMountain)
                    {
                        statHighestMountain = cell.Elevation;
                    }
                    if(cell.Elevation < statLowestPoint)
                    {
                        // Now all data has been set, calculate the modifiers
                        statLowestPoint = cell.Elevation;
                    }

                    // Set wetness by rain
                    var rain = perlin.OctavePerlin(perlinPoint.X, perlinPoint.Y, perlinZ * 100, 2, 0.7) * scene.Settings.RainMultiplier;
                    cell.Wetness = rain;
                }
            }

            // Creates oceans by flood filling from map edge
            FloodFillOceans();

            // Do final touchups and modifier calculation
            // TODO: This can contain NULL cells, this is bad
            var cells = scene.WorldMap.Cells;
            foreach(var cell in cells)
            {
                if(cell == null) { continue; }

                // Remove single-cell 'seas' and islands
                if(cell.IsWater)
                {
                    bool canBeWater = false;
                    foreach(var n in cell.Neighbours)
                    {
                        if(n.IsWater) { canBeWater = true; break; }
                    }
                    cell.IsWater = canBeWater;
                }
                else
                {
                    bool canBeLand = false;
                    foreach(var n in cell.Neighbours)
                    {
                        if(!n.IsWater) { canBeLand = true; break; }
                    }
                    cell.IsWater = !canBeLand;
                }

                // Calculate temperature
                var distanceFromEdge = (radius - TopographyHelper.Distance(new Point(0, 0), cell.Coordinates)) / radius;
                var temperature = MathHelper.Lerp(scene.Settings.PoleTemperature, scene.Settings.EquatorTemperature,
                    (float)Math.Pow(distanceFromEdge, scene.Settings.TemperatureCurvePower));
                var relElevation = (float)cell.Elevation - scene.Settings.SeaLevel;
                if(!cell.IsWater)
                {
                    // Ocean tiles should have temperature at sea level only, otherwise use altitude multiplier
                    temperature += relElevation * scene.Settings.ElevationTemperatureMultiplier;
                }
                cell.Temperature = temperature;

                // Set values
                if (cell.IsWater == true)
                {
                    statSeaTiles++;
                    cell.FoodMod = random.NextDouble() + random.Next(1, 2);
                    cell.ResourceMod = random.NextDouble() + random.Next(1, 2);
                    cell.MaxHousing = 0;
                }
                else
                {
                    // Now all data has been set, calculate the modifiers
                    cell.FoodMod = random.NextDouble() + random.Next(1, 5);
                    cell.ResourceMod = random.NextDouble() + random.Next(1, 5);
                    cell.WealthMod = random.NextDouble() + random.Next(1, 5);

                    cell.MaxHousing = random.Next(0, 1250) + random.Next(0, 1250) + random.Next(0, 1250) + random.Next(0, 1250);
                }

                // Set biome
                AssignBiome(cell);
            }

            //SimulateWaterflow();

            // Log stats
            Debug.WriteLine("Stats for World Generation");
            Debug.WriteLine("Highest Point: " + statHighestMountain);
            Debug.WriteLine("Lowest Point:  " + statLowestPoint);
            Debug.WriteLine("Ocean Cells:   " + statSeaTiles);
            Debug.WriteLine("Land Cells:    " + (scene.WorldMap.CellCount - statSeaTiles));
            Debug.WriteLine("Water coverage:" + ((float)statSeaTiles / scene.WorldMap.CellCount * 100).ToString("##.##") + "%");
        }

        private void FloodFillOceans()
        {
            var cell = scene.WorldMap.GetCell(scene.WorldMap.Radius, 0);
            var cellsToCheck = new Queue<Cell>();
            cellsToCheck.Enqueue(cell);
            do
            {
                cell = cellsToCheck.Dequeue();
                cell.IsWater = true;
                foreach (var n in cell.Neighbours)
                {
                    if (!n.IsWater && n.Elevation <= scene.Settings.SeaLevel && !cellsToCheck.Contains(n))
                    {
                        cellsToCheck.Enqueue(n);
                    }
                }
            } while (cellsToCheck.Count > 0);
        }

        private void SimulateWaterflow()
        {
            // Sort cells by height
            var heightSortedCells = new List<Cell>();
            foreach(var cell in scene.WorldMap.Cells)
            {
                if(cell != null && !cell.IsWater)
                {
                    heightSortedCells.Add(cell);
                }
            }
            heightSortedCells = heightSortedCells.OrderByDescending(c => c.Elevation).ToList();

            // Make water go down
            foreach(var cell in heightSortedCells)
            {
                var elevation = cell.Elevation;
                Cell targetCell = null;
                foreach (var n in cell.Neighbours)
                {
                    if (n.Elevation < elevation)
                    {
                        elevation = n.Elevation;
                        targetCell = n;
                    }
                }
                if (targetCell != null)
                {
                    targetCell.Wetness += cell.Wetness;
                }
                else
                {
                    // Try to erode? Form a lake?
                }

                // TEST: Turn cells into water cells above threshold
                /*if(cell.Wetness >= scene.Settings.LakeWetness)
                {
                    cell.IsWater = true;
                }
                else if (cell.Wetness >= scene.Settings.RiverWetness)
                {
                }*/
            }
        }

        private void AssignBiome(Cell cell)
        {
            // Right now these values are hardcoded
            // Maps should be 128*32
            // X:
            // 63 = 0 *C
            // 0 = -63 *C
            // 126 = +63 *C
            // 127 = +64 *C
            // Y:
            // 0 = 0 mm rain
            // 31 = 3100 mm rain
            int x = MathHelper.Clamp((int)Math.Round(cell.Temperature) + 63, 0, 127);
            int y = MathHelper.Clamp((int)Math.Round(cell.Wetness / 100), 0, 31);
            if(cell.IsWater)
            {
                cell.Biome = scene.BiomeCollection.GetSeaBiome(x, y);
            }
            else
            {
                cell.Biome = scene.BiomeCollection.GetLandBiome(x, y);
            }

            cell.FoodMod *= cell.Biome.FoodModifier;
            cell.ResourceMod *= cell.Biome.ResourceModifier;
            cell.MaxHousing = (int)(cell.MaxHousing * cell.Biome.PopulationModifier);
        }
    }
}
