using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSim
{
    public class World : IWorld
    {
        private ICell[,,] cells;
               

        public ICell[,,] Cell
        {
            get { return cells; }
        }

        public World()
        {
            cells = new ICell[10, 10, 10];
        }

        public World(int x, int y, int z)
        {
            cells = new ICell[z, y, x];
        }
    }
}
