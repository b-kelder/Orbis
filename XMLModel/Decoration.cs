using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLModel
{
    public enum DecorationType
    {
        Default,
        SmallPopulation,
        MediumPopulation,
        LargePopulation,
    }
    /// <summary>
    /// A collection of decorations and some extra data
    /// </summary>
    public class DecorationCollection
    {
        public int SmallPopulationThreshold { get; set; }
        public int MediumPopulationThreshold { get; set; }
        public int LargePopulationThreshold { get; set; }
        public List<Decoration> Decorations { get; set; }
    }
    /// <summary>
    /// Defines a cell decoration that can be rendered on a cell
    /// </summary>
    public class Decoration
    {
        public string Name { get; set; }
        public DecorationType Type { get; set; }
        public Model Model { get; set; }
    }
}
