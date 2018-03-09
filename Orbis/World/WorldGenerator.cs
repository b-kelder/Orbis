using Microsoft.Xna.Framework;
using Orbis.Simulation;
using Orbis.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orbis.World
{
    class WorldGenerator
    {
        public string[] names =
        {
            "Andorra",
        "United Arab Emirates",
        "Afghanistan",
        "Antigua and Barbuda",
        "Albania",
        "Armenia",
        "Angola",
        "Argentina",
        "Austria",
        "Australia",
        "Azerbaijan",
        "Bosnia and Herzegovina",
        "Barbados",
        "Bangladesh",
        "Belgium",
        "Burkina Faso",
        "Bulgaria",
        "Bahrain",
        "Burundi",
        "Benin",
        "Brunei Darussalam",
        "Bolivia (Plurinational State of)",
        "Brazil",
        "Bahamas",
        "Bhutan",
        "Botswana",
        "Belarus",
        "Belize",
        "Canada",
        "Democratic Republic of the Congo",
        "Central African Republic",
        "Congo",
        "Switzerland",
        "Côte d'Ivoire",
        "Chile",
        "Cameroon",
        "China",
        "Colombia",
        "Costa Rica",
        "Cuba",
        "Cape Verde",
        "Cyprus",
        "Czech Republic",
        "Germany",
        "Djibouti",
        "Denmark",
        "Dominica",
        "Dominican Republic",
        "Algeria",
        "Ecuador",
        "Estonia",
        "Egypt",
        "Eritrea",
        "Spain",
        "Ethiopia",
        "Finland",
        "Fiji",
        "Micronesia (Federated States of)",
        "France",
        "Gabon",
        "United Kingdom of Great Britain and Northern Ireland",
        "Grenada",
        "Georgia",
        "Ghana",
        "Gambia",
        "Guinea",
        "Equatorial Guinea",
        "Greece",
        "Guatemala",
        "Guinea-Bissau",
        "Guyana",
        "Honduras",
        "Croatia",
        "Haiti",
        "Hungary",
        "Indonesia",
        "Ireland",
        "Israel",
        "India",
        "Iraq",
        "Iran (Islamic Republic of)",
        "Iceland",
        "Italy",
        "Jamaica",
        "Jordan",
        "Japan",
        "Kenya",
        "Kyrgyzstan",
        "Cambodia",
        "Kiribati",
        "Comoros",
        "Saint Kitts and Nevis",
        "Democratic People's Republic of Korea",
        "Republic of Korea",
        "Kuwait",
        "Kazakhstan",
        "Lao People's Democratic Republic",
        "Lebanon",
        "Saint Lucia",
        "Liechtenstein",
        "Sri Lanka",
        "Liberia",
        "Lesotho",
        "Lithuania",
        "Luxembourg",
        "Latvia",
        "Libyan Arab Jamahiriya",
        "Morocco",
        "Monaco",
        "Republic of Moldova",
        "Montenegro",
        "Madagascar",
        "Marshall Islands",
        "The former Yugoslav Republic of Macedonia",
        "Mali",
        "Myanmar",
        "Mongolia",
        "Mauritania",
        "Malta",
        "Mauritius",
        "Maldives",
        "Malawi",
        "Mexico",
        "Malaysia",
        "Mozambique",
        "Namibia",
        "Niger",
        "Nigeria",
        "Nicaragua",
        "Netherlands",
        "Norway",
        "Nepal",
        "Nauru",
        "New Zealand",
        "Oman",
        "Panama",
        "Peru",
        "Papua New Guinea",
        "Philippines",
        "Pakistan",
        "Poland",
        "Portugal",
        "Palau",
        "Paraguay",
        "Qatar",
        "Romania",
        "Serbia",
        "Russian Federation",
        "Rwanda",
        "Saudi Arabia",
        "Solomon Islands",
        "Seychelles",
        "Sudan",
        "Sweden",
        "Singapore",
        "Slovenia",
        "Slovakia",
        "Sierra Leone",
        "San Marino",
        "Senegal",
        "Somalia",
        "Suriname",
        "South Sudan",
        "Sao Tome and Principe",
        "El Salvador",
        "Syrian Arab Republic",
        "Swaziland",
        "Chad",
        "Togo",
        "Thailand",
        "Tajikistan",
        "Timor-Leste",
        "Turkmenistan",
        "Tunisia",
        "Tonga",
        "Turkey",
        "Trinidad and Tobago",
        "Tuvalu",
        "United Republic of Tanzania",
        "Ukraine",
        "Uganda",
        "United States of America",
        "Uruguay",
        "Uzbekistan",
        "Saint Vincent and the Grenadines",
        "Venezuela (Bolivarian Republic of)",
        "Viet Nam",
        "Vanuatu",
        "Samoa",
        "Yemen",
        "South Africa",
        "Zambia",
        "Zimbabwe"
        };

        /// <summary>
        /// Random object
        /// </summary>
        private Random random;
        public float SeaLevel { get; set; }
        public float MaxElevation { get; set; }

        /// <summary>
        /// World generator constructor
        /// </summary>
        /// <param name="seed">A seed to base generation on</param>
        public WorldGenerator(int seed)
        {
            // Create a random object based on a seed
            random = new Random(seed);
            SeaLevel = 8;
            MaxElevation = 15;
        }

        public void GenerateCivs(Scene scene, int count)
        {
            scene.Civilizations = new List<Civilization>();
            for(int i = 0; i < count; i++)
            {
                // Create a civ with all base values
                Civilization civ = new Civilization
                {
                    Name = names[random.Next(names.Length)],
                    DefenceModifier = Dice.Roll(6, 1),
                    OffenceModifier = Dice.Roll(6, 1),
                    Population = 1,
                    TechnologicalProgress = 0,
                    Wealth = 0,
                };

                // Select a random starting cell for the civ
                int x, y;
                // Loop until no cell is available or until break
                while(scene.Civilizations.Count < scene.WorldMap.CellCount)
                {
                    // Get random X and Y coordinates
                    // TODO: Make random function that always gives tile within radius?
                    x = random.Next(-scene.WorldMap.Radius, scene.WorldMap.Radius);
                    y = random.Next(-scene.WorldMap.Radius, scene.WorldMap.Radius);

                    // Check if the cell has an owner
                    var cell = scene.WorldMap.GetCell(x, y);
                    if(cell != null && cell.Owner == null)
                    {
                        // No atlantis shenanigans
                        if(cell.IsWater)
                        {
                            continue;
                        }
                        // Set the owner and break
                        civ.Territory.Add(cell);
                        cell.Owner = civ;

                        break;
                    }
                }

                // Add the civ to the world
                scene.Civilizations.Add(civ);
            }
        }
        
        public void GenerateWorld(Scene scene, int radius)
        {
            Perlin perlin = new Perlin(5);
            float boundsX = TopographyHelper.HexToWorld(new Point(radius, 0)).X;
            float boundsY = TopographyHelper.HexToWorld(new Point(0, radius)).Y;
            var perlinZ = random.NextDouble();

            scene.WorldMap = new Map(radius);

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
                    cell.Elevation = perlin.OctavePerlin(perlinPoint.X, perlinPoint.Y, perlinZ, 10, 0.7) * MaxElevation;
                    if(cell.Elevation <= SeaLevel)
                    {
                        cell.IsWater = true;
                    }
                    else
                    {
                        // Now all data has been set, calculate the modifiers
                        cell.FoodMod = random.NextDouble() + random.Next(5);
                        cell.ResourceMod = random.NextDouble();
                        cell.Housing = random.Next(1, 100);
                    }
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
                    scene.WorldMap[x, y] = new Cell(perlin.OctavePerlin((x + 0.5) / 1000, (y + 0.5) / 1000, seed, 10, 0.7));

                    //Debug.WriteLine("X:" + x + " Y:" + y + " NV:" + scene.WorldMap[x, y].NoiseValue);
                }
            }

            // Set neighbours for each cell
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Cell cell = scene.WorldMap[x, y];
                    // Create a list to store the cells
                    cell.Neighbours = new List<Cell>();

                    // Check if a right neighbour exists
                    if (x + 1 < sizeX)
                    {
                        cell.Neighbours.Add(scene.WorldMap[x + 1, y]);
                    }
                    // Check if a bottom right neighbour exists
                    if (y + 1 < sizeY)
                    {
                        cell.Neighbours.Add(scene.WorldMap[x, y + 1]);
                    }
                    // Check if a bottom left neighbour exists
                    if (x - 1 > sizeX && y + 1 < sizeY)
                    {
                        cell.Neighbours.Add(scene.WorldMap[x - 1, y + 1]);
                    }
                    // Check if a top right neighbour exists
                    if (x + 1 < sizeX && y - 1 > sizeY)
                    {
                        cell.Neighbours.Add(scene.WorldMap[x + 1, y - 1]);
                    }
                    // Check if a left neighbour exists
                    if (x - 1 > sizeX)
                    {
                        cell.Neighbours.Add(scene.WorldMap[x - 1, y]);
                    }
                    // Check if a top left neighbour exists
                    if (y - 1 > sizeY)
                    {
                        cell.Neighbours.Add(scene.WorldMap[x, y - 1]);
                    }

                    // Now all data has been set, calculate the modifiers
                    cell.FoodMod = random.NextDouble();
                    cell.ResourceMod = random.NextDouble();
                    cell.Housing = random.Next(1, 100);
                }
            }
        }*/
    }
}