using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLModel
{
    public class Model
    {
        /// <summary>
        /// Name of the model file
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Name of the texture file
        /// </summary>
        public string Texture { get; set; }
        /// <summary>
        /// Name of the color texture file
        /// </summary>
        public string ColorTexture { get; set; }
    }
}
