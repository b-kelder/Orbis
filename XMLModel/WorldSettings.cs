using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLModel
{
    public class WorldSettings
    {
        public float SeaLevel;
        /// <summary>
        /// Rainfall [0-1] gets multiplied by this to get mm of rain per year.
        /// </summary>
        public float RainMultiplier;
        public float RiverWetness;
        public float LakeWetness;
        public float ElevationMultiplier;
        public float EdgeDistancePower;
        public float ElevationPower;
        /// <summary>
        /// Temperature at the 'poles' (map edges), degrees Celcius
        /// </summary>
        public float PoleTemperature;
        /// <summary>
        /// Temperature at the equator (map center), degrees Celcius
        /// </summary>
        public float EquatorTemperature;
        /// <summary>
        /// Used for the interpolation curve for pole-equator distance based temperature.
        /// 1 is linear.
        /// </summary>
        public float TemperatureCurvePower;
        /// <summary>
        /// Linear multiplier for temperature based on elevation.
        /// Temperatures are based around SeaLevel, so no need to manually offset.
        /// </summary>
        public float ElevationTemperatureMultiplier;
    }
}
