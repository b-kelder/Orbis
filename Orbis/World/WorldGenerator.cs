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

        /// <summary>
        /// World generator constructor
        /// </summary>
        /// <param name="seed">A seed to base generation on</param>
        public WorldGenerator(int seed)
        {
            // Create a random object based on a seed
            random = new Random(seed);
        }

        /// <summary>
        /// Generate and place civs on the world map
        /// A world map needs to be generated before this function can be called
        /// </summary>
        /// <param name="scene">The scene to generate for</param>
        /// <param name="amount">The amount of civs to generate</param>
        public void GenerateCivs(Scene scene, int amount)
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

        /// <summary>
        /// Generate a hex grid world map with a specified size
        /// </summary>
        /// <param name="scene">The scene to generate the world for</param>
        /// <param name="sizeX">The size in X</param>
        /// <param name="sizeY">The size in Y</param>
        public void GenerateWorld(Scene scene, int sizeX, int sizeY)
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
        }
    }
}
