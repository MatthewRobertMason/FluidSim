using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSim
{
    public interface ICell
    {
        Guid ID { get; set; }
        Dictionary<int, double> Gasses { get; set; }
        // Temperature
        List<ICell> Neighbours{get; set;}
        Guid OwningPocket { get; set; }
    }
}
