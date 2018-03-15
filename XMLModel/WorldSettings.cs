using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLModel
{
    public class WorldSettings
    {
        public float SeaLevel { get; set; }
        /// <summary>
        /// Rainfall [0-1] gets multiplied by this to get mm of rain per year.
        /// </summary>
        public float RainMultiplier { get; set; }
        public float RiverWetness { get; set; }
        public float LakeWetness { get; set; }
        public float ElevationMultiplier { get; set; }
        public float EdgeDistancePower { get; set; }
        public float ElevationPower { get; set; }
        /// <summary>
        /// Temperature at the 'poles' (map edges), degrees Celcius
        /// </summary>
        public float PoleTemperature { get; set; }
        /// <summary>
        /// Temperature at the equator (map center), degrees Celcius
        /// </summary>
        public float EquatorTemperature { get; set; }
        /// <summary>
        /// Used for the interpolation curve for pole-equator distance based temperature.
        /// 1 is linear.
        /// </summary>
        public float TemperatureCurvePower { get; set; }
        /// <summary>
        /// Linear multiplier for temperature based on elevation.
        /// Temperatures are based around SeaLevel, so no need to manually offset.
        /// </summary>
        public float ElevationTemperatureMultiplier { get; set; }
    }
}
