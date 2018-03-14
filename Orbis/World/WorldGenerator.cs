using Microsoft.Xna.Framework;
using Orbis.Simulation;
using Orbis.Engine;
using System;
using System.Collections.Generic;

namespace Orbis.World
{
    class WorldGenerator
    {
        /// <summary>
        /// Random object
        /// </summary>
        private Random random;
        public float SeaLevel { get; set; }
        public float MaxElevation { get; set; }

        private Scene scene;

        /// <summary>
        /// World generator constructor
        /// </summary>
        /// <param name="seed">A seed to base generation on</param>
        public WorldGenerator(Scene scene)
        {
            this.scene = scene;

            // Create a random object based on a seed
            random = new Random(scene.Seed);
            SeaLevel = 18;
            MaxElevation = 35;
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
                Civilization civ = new Civilization
                {
                    Name = "Kees" + i,
                };

                // Select a random starting cell for the civ
                // Loop until no cell is available or until break
                while(scene.Civilizations.Count < scene.WorldMap.CellCount && availableCells.Count > 0)
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
                        if(!civ.ClaimCell(cell))
                        {
                            continue;
                        }

                        cell.population = 1;

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


        /// <summary>
        /// Generate and place civs on the world map
        /// A world map needs to be generated before this function can be called
        /// </summary>
        /// <param name="scene">The scene to generate for</param>
        /// <param name="amount">The amount of civs to generate</param>
        /*public void GenerateCivs(Scene scene, int amount)
        {
            if (scene.WorldMap.Length <= 0)
            {
                throw new Exception("Worldmap not generated exception");
            }

            // Create a list of civs
            scene.Civilizations = new List<Civilization>();

            // Go through generation for all civs
            for (int i = 0; i < amount; i++)
            {
                // Create a civ with all base values
                Civilization civ = new Civilization
                {
                    DefenceModifier = Dice.Roll(6, 1),
                    OffenceModifier = Dice.Roll(6, 1),
                    Population = 1,
                    TechnologyProgress = 0,
                    Wealth = 0,
                };

                // Select a random starting cell for the civ
                int x, y;
                // Loop until no cell is available or until break
                while (scene.Civilizations.Count < scene.WorldMap.Length)
                {
                    // Get random X and Y coordinates
                    x = random.Next(scene.WorldMap.GetLength(0));
                    y = random.Next(scene.WorldMap.GetLength(1));

                    // Check if the cell has an owner
                    if (scene.WorldMap[x, y].Owner == null)
                    {
                        if (scene.WorldMap[x,y].NoiseValue < 0.45 || scene.WorldMap[x, y].NoiseValue > 0.52)
                        {
                            continue;
                        }
                        // Set the owner and break
                        civ.Territory.Add(scene.WorldMap[x, y]);
                        scene.WorldMap[x, y].Owner = civ;

                        break;
                    }
                }

                // Add the civ to the world
                scene.Civilizations.Add(civ);
            }
        }
        */
        public void GenerateWorld(int radius)
        {
            Perlin perlin = new Perlin(5);
            float boundsX = TopographyHelper.HexToWorld(new Point(radius, 0)).X;
            float boundsY = TopographyHelper.HexToWorld(new Point(0, radius)).Y;
            var perlinZ = random.NextDouble();

            scene.WorldMap = new Map(radius);
            scene.WorldMap.SeaLevel = SeaLevel;

            for(int p = -radius; p <= radius; p++)
            {
                for(int q = -radius; q <= radius; q++)
                {
                    var cell = scene.WorldMap.GetCell(p, q);
                    if(cell == null)
                    {
                        continue;
                    }

                    // Set cell neighbors
                    cell.Neighbours = scene.WorldMap.GetNeighbours(new Point(p, q));

                    // Set cell height
                    var worldPoint = TopographyHelper.HexToWorld(new Point(p, q));
                    var perlinPoint = (worldPoint + new Vector2(boundsX, boundsY)) * 0.01f;
                    cell.Elevation = perlin.OctavePerlin(perlinPoint.X, perlinPoint.Y, perlinZ, 4, 0.7) * MaxElevation;
                    if (cell.Elevation <= SeaLevel)
                    {
                        cell.IsWater = true;
                    }
                    else
                    {
                        // Now all data has been set, calculate the modifiers
                        cell.FoodMod = random.NextDouble() + random.Next(5);
                        cell.ResourceMod = random.NextDouble();
                        cell.MaxHousing = random.Next(1, 5000);
                    }
                }
            }

            // Loop trough cells
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
            }
        }

        /// <summary>
        /// Generate a hex grid world map with a specified size
        /// </summary>
        /// <param name="scene">The scene to generate the world for</param>
        /// <param name="sizeX">The size in X</param>
        /// <param name="sizeY">The size in Y</param>
        /*public void GenerateWorld(Scene scene, int sizeX, int sizeY)
        {
            Perlin perlin = new Perlin(5);

            // Create a array to store cells
            scene.WorldMap = new Cell[sizeX, sizeY];

            double seed = random.NextDouble();

            // Generate cells
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    scene.WorldMap[x, y] = new Cell(perlin.OctavePerlin((x+0.5) / 1000, (y+0.5) / 1000, seed, 10, 0.7));

                    //Debug.WriteLine("X:" + x + " Y:" + y + " NV:" + scene.WorldMap[x, y].NoiseValue);
                }
            }

            // Set neighbours for each cell
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    // Create a list to store the cells
                    scene.WorldMap[x, y].Neighbours = new List<Cell>();

                    // Check if a right neighbour exists
                    if (x + 1 < sizeX)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x + 1, y]);
                    }
                    // Check if a bottom right neighbour exists
                    if (y + 1 < sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x, y + 1]);
                    }
                    // Check if a bottom left neighbour exists
                    if (x - 1 > sizeX && y + 1 < sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x - 1, y + 1]);
                    }
                    // Check if a top right neighbour exists
                    if (x + 1 < sizeX && y - 1 > sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x + 1, y - 1]);
                    }
                    // Check if a left neighbour exists
                    if (x - 1 > sizeX)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x - 1, y]);
                    }
                    // Check if a top left neighbour exists
                    if (y - 1 > sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x, y - 1]);
                    }

                    // Set the biome for the cell based on heightmap
                    // TODO: Set the biome for the cell based on heightmap
                    scene.WorldMap[x, y].Biome = null;

                    // Now all data has been set, calculate the modifiers
                    scene.WorldMap[x, y].CalculateModifiers();
                }
            }
        }*/
    }
}
