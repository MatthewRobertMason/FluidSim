using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSim
{
    class Gas
    {
        // Enum
        public static int numberOfGasses = 2;

        public enum GasType
        {
            AIR,
            KEROSENE
        }


        // Class
        public const double MIN_PRESSURE = 0.001;
        public const double MAX_PRESSURE = 10000.0;
        public const double PRECISION = 0.1;

        public double pressure;
        public GasType type;

        public Gas()
        {
            pressure = 0.0;
        }
    }
}
