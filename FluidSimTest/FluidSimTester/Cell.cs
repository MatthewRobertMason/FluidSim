using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace FluidSim
{
    public class Cell : ICell
    {
        public Guid ID { get; set; }
        public Dictionary<int, double> Gasses { get; set; }

        //public Dictionary<int, float> Gasses { get; set; }

        //private List<Cell> neighbours;
        public List<ICell> Neighbours
        {
            get;
            set;
        }

        public Guid OwningPocket { get; set; }

        public Cell()
        {
            Awake();
        }

        // Use this for initialization
        void Awake()
        {
            ID = Guid.NewGuid();
            Gasses = new Dictionary<int, double>();
            Neighbours = new List<ICell>();
            OwningPocket = Guid.Empty;
        }
    }
}
