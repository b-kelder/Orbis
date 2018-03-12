
namespace XMLModel
{
    public class Biome
    {
        public string name = "unknown";
        /// <summary>
        /// Modifier for the amount of food within the biome used as a basis for cell modifiers
        /// </summary>
        public int foodModifier = 0;
        /// <summary>
        /// Modifier for the amount of recources in the biome  used as a basis for cell modifiers
        /// </summary>
        public int resourceModifier = 0;
        /// <summary>
        /// Modifier for the amount of usable space for the population  used as a basis for cell modifiers
        /// </summary>
        public int populationModifier = 0;
    }
}
