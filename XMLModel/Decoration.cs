using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLModel
{
    /// <summary>
    /// A collection of decorations and some extra data
    /// </summary>
    public class DecorationCollection
    {
        public int SmallPopulationThreshold;
        public int MediumPopulationThreshold;
        public int LargePopulationThreshold;
        public List<Decoration> Decorations;
    }
    /// <summary>
    /// Defines a cell decoration that can be rendered on a cell
    /// </summary>
    public class Decoration
    {
        public string Name;
        public Model Model;
    }
}
