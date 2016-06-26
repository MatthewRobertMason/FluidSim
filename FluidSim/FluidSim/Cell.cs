using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSim
{
    class Cell
    {
        public Dictionary<Gas.GasType, Gas> gasses;
        public List<Cell> neighbours;
        public Guid owningPocket;

        public Cell()
        {
            gasses = new Dictionary<Gas.GasType, Gas>();
            neighbours = new List<Cell>();
            owningPocket = Guid.Empty;
        }
    }
}
