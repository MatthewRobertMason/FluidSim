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

        public Cell()
        {
            gasses = new Dictionary<Gas.GasType, Gas>();
            neighbours = new List<Cell>();
        }

        /// <summary>
        /// Equalizes the two cells
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns>
        /// Returns true if the cells didn't change
        /// </returns>
        public bool Equalize(Cell rhs)
        {
            bool equalized = false;
            List<Gas.GasType> gasTypes = new List<Gas.GasType>();
            foreach (Gas.GasType g in gasses.Keys)
            {
                gasTypes.Add(g);
            }

            foreach (Gas.GasType g in gasTypes)
            {
                // Make sure it still contains this gas
                if (this.gasses.ContainsKey(g)) 
                {
                    if (!rhs.gasses.ContainsKey(g))
                    {
                        rhs.gasses.Add(g, new Gas() { type = g, pressure = 0.0 });
                    }

                    if (Math.Abs(this.gasses[g].pressure - rhs.gasses[g].pressure) <= Gas.PRECISION)
                    {
                        equalized = true;
                    }
                    else
                    {
                        this.gasses[g].pressure = (this.gasses[g].pressure + rhs.gasses[g].pressure) / 2.0;

                        if (this.gasses[g].pressure < Gas.MIN_PRESSURE)
                        {
                            equalized = false;
                            this.gasses.Remove(g);
                            rhs.gasses.Remove(g);
                        }
                        else
                        {
                            if (this.gasses[g].pressure > Gas.MAX_PRESSURE)
                                this.gasses[g].pressure = Gas.MAX_PRESSURE;

                            rhs.gasses[g].pressure = this.gasses[g].pressure;

                            equalized = false;
                        }
                    }
                }
            }

            return equalized;
        }
    }
}
