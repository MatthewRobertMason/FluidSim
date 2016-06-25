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

        /// <summary>
        /// Equalizes the two cells
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns>
        /// Returns true if the cell didn't change
        /// </returns>
        public List<Cell> Equalize(List<Cell> neighbours, Gas.GasType gasType)
        {
            bool equalized = false;
            double initPresure = this.gasses[gasType].pressure;
            double pressure = this.gasses[gasType].pressure;
            double[] initPressures = new double[neighbours.Count];
            List<Cell> nonEqualized = new List<Cell>();

            

            //foreach (Cell c in neighbours)
            //{
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (!neighbours[i].gasses.ContainsKey(gasType))
                {
                    neighbours[i].gasses.Add(gasType, new Gas() { type = gasType, pressure = 0.0 });
                }

                initPressures[i] = neighbours[i].gasses[gasType].pressure;
                pressure += neighbours[i].gasses[gasType].pressure;
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
                gasses[gasType].pressure = pressure;
                foreach (Cell c in neighbours)
                {
                    c.gasses[gasType].pressure = pressure;
                }

                //equalized = (initPresure == pressure);
                equalized = (Math.Abs(initPresure - pressure) <= Gas.PRECISION);

                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (Math.Abs(initPressures[i] - pressure) > Gas.PRECISION)
                    {
                        nonEqualized.Add(neighbours[i]);
                        equalized = false;
                    }
                }
            }
            return nonEqualized;
        }
    }
}
