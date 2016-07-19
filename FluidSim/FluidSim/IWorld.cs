using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSim
{
    public interface IWorld
    {
        ICell[,,] Cell { get; }
    }
}
