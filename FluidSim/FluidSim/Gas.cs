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

        public double pressure;
        public GasType type;

        public Gas()
        {
            pressure = 0.0;
        }
    }
}
