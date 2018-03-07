using Orbis.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.World
{
    class WorldGenerator
    {
        private Random random;

        public WorldGenerator(int seed)
        {
            random = new Random(seed);
        }

        public void GenerateCivs(Scene scene, int amount)
        {
            scene.Civilizations = new List<Civilization>();
            for (int i = 0; i < amount; i++)
            {
                Civilization civ = new Civilization
                {
                    DefenceModifier = Dice.Roll(6, 1),
                    OffenceModifier = Dice.Roll(6, 1),
                    Population = 1,
                    TechnologyProgress = 0,
                    Wealth = 0,
                };

                int x, y;

                while (scene.Civilizations.Count < scene.WorldMap.Length)
                {
                    x = random.Next(scene.WorldMap.GetLength(0));
                    y = random.Next(scene.WorldMap.GetLength(1));

                    if (scene.WorldMap[x, y].Owner == null)
                    {
                        civ.Territory.Add(scene.WorldMap[x, y]);
                        scene.WorldMap[x, y].Owner = civ;
                        break;
                    }
                }

                scene.Civilizations.Add(civ);
            }
        }

        public void GenerateWorld(Scene scene, int sizeX, int sizeY)
        {
            scene.WorldMap = new Cell[sizeX, sizeY];

            // Generate cells
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    scene.WorldMap[x, y] = new Cell();
                }
            }

            // Set neighbours
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    scene.WorldMap[x, y].Neighbours = new List<Cell>();

                    if (x + 1 <= sizeX)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x + 1, y]);
                    }
                    if (y + 1 <= sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x, y + 1]);
                    }
                    if (x - 1 >= sizeX && y + 1 <= sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x - 1, y + 1]);
                    }
                    if (x + 1 <= sizeX && y - 1 >= sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x + 1, y - 1]);
                    }
                    if (x - 1 >= sizeX)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x - 1, y]);
                    }
                    if (y - 1 >= sizeY)
                    {
                        scene.WorldMap[x, y].Neighbours.Add(scene.WorldMap[x, y - 1]);
                    }

                    scene.WorldMap[x, y].CalculateModifiers();
                }
            }
        }
    }
}
