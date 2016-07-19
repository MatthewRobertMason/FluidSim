using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace FluidSim
{
    public class Gas
    {
        // This class might be removable

        // TODO: This is being moved to an array in FluidManager
        //<REMOVE>
        // Enum
        public static int numberOfGasses = 2;   // TODO: Move this to World
        public enum GasType
        {
            AIR,
            KEROSENE
        }
        // </REMOVE>

        public double pressure;
        public int type;    // Gastypes are now stored in an array in FluidManager

        public Gas()
        {
            pressure = 0.0;
        }
    }
}
