using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSim
{
    class FluidManager
    {
        public const int MAXNUMBER_TO_PROCESS = 600;
        public const int MAX_NUMBER_OF_VOID_TO_PROCESS = 6000;
        Queue<Cell> cells;
        Queue<Cell> voidCells;

        Cell currentCell;
        public Cell voidCell;

        public FluidManager()
        {
            cells = new Queue<Cell>();
            voidCells = new Queue<Cell>();
        }

        public void Add(Cell c)
        {
            if (!cells.Contains(c))
                cells.Enqueue(c);
        }

        /// <summary>
        /// Process Fluid simulation for a given number of cells
        /// </summary>
        /// <returns>
        /// returns true if there are more cells to process
        /// </returns>
        public bool ProcessNodes()
        {
            int ammount = Math.Min(MAXNUMBER_TO_PROCESS, cells.Count);
            bool equalized = true;

            if (ammount == 0)
                return false;

            for (int i = 0; i < ammount; i++)
            {
                currentCell = cells.Dequeue();

                foreach (Cell c in currentCell.neighbours)
                {
                    /*
                    if (c == voidCell)
                    {
                        if (!voidCells.Contains(currentCell))
                        {
                            voidCells.Enqueue(currentCell);
                        } 
                        //equalized = true; 
                    }
                    else */
                    if (currentCell.Equalize(c))
                    {
                        equalized = true;
                    }
                    else
                    {
                        // If the cell wasn't equalized then add it for processing 
                        //if (!cells.Contains(c))
                            cells.Enqueue(c);
                        //equalized = false;
                    }
                }

                // If this cell isn't equalized then add it to be reprocessed
                if (!equalized)
                {
                   // if (!cells.Contains(currentCell))
                        cells.Enqueue(currentCell);
                }
            }
            /*
            // Process void cells
            for (int i = 0; ((i < MAX_NUMBER_OF_VOID_TO_PROCESS) && (voidCells.Count > 0)); i++)
            {
                currentCell = voidCells.Dequeue();

                foreach (Cell c in currentCell.neighbours)
                {
                    if (c != voidCell)
                    {
                        if (c.gasses.Count > 0)
                        {
                            c.gasses.Clear();
                            voidCells.Enqueue(c);
                        }
                    }
                }
                currentCell.gasses.Clear();
            }
                */
            return true;
        }
    }
}
