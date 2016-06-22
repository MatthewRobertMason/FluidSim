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
        /// Returns true if the cell didn't change
        /// </returns>
        public bool Equalize(List<Cell> neighbours, Gas.GasType gasType)
        {
            bool equalized = false;
            double initPresure = this.gasses[gasType].pressure;
            double pressure = this.gasses[gasType].pressure;

            foreach (Cell c in neighbours)
            {
                if (!c.gasses.ContainsKey(gasType))
                {
                    c.gasses.Add(gasType, new Gas() { type = gasType, pressure = 0.0 });
                }

                pressure += c.gasses[gasType].pressure;
            }

            pressure /= (neighbours.Count + 1);
            
            if (pressure > Gas.MAX_PRESSURE)
                pressure = Gas.MAX_PRESSURE;

            if (pressure < Gas.MIN_PRESSURE)
            {
                pressure = 0.0d;
                this.gasses.Remove(gasType);
                foreach (Cell c in neighbours)
                {
                    c.gasses.Remove(gasType);
                }

                equalized = true;
            }
            else
            {
                foreach (Cell c in neighbours)
                {
                    c.gasses[gasType].pressure = pressure;
                }

                //equalized = (initPresure == pressure);
                equalized = (Math.Abs(initPresure - pressure) <= Gas.PRECISION);

            }
            /*
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
            */
            return equalized;
        }
    }
}
