using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSim
{
    class FluidManager
    {
        public const int MAX_NUMBER_TO_PROCESS = 600000000;
        //public const int MAX_NUMBER_OF_VOID_TO_PROCESS = 6000;

        HashSet<Cell> cells; // Cells requiring updating

        //LinkedList<Guid> pocketsToUpdate; // gas pockets to update
        HashSet<Guid> pocketsToUpdate;
        Dictionary<Guid, HashSet<Cell>> gasPockets;
        Dictionary<Guid, HashSet<Cell>> visitedGasPockets; // used in expand gas pocket to keep track of visited cells for current and following cells
        //Dictionary<Guid, HashSet<Cell>> unvisitedGasPocketCells;
        //Dictionary<Guid, LinkedList<Cell>> unvisitedGasPocketCells;
        Dictionary<Guid, Queue<Cell>> unvisitedGasPocketCells;

        //Cell currentCell;
        public Cell voidCell;

        public FluidManager()
        {
            cells = new HashSet<Cell>();
            visitedGasPockets = new Dictionary<Guid,HashSet<Cell>>();
            //unvisitedGasPocketCells = new Dictionary<Guid, HashSet<Cell>>();
            //unvisitedGasPocketCells = new Dictionary<Guid, LinkedList<Cell>>();
            unvisitedGasPocketCells = new Dictionary<Guid, Queue<Cell>>();
            gasPockets = new Dictionary<Guid, HashSet<Cell>>();
            //pocketsToUpdate = new LinkedList<Guid>();
            pocketsToUpdate = new HashSet<Guid>();
        }

        /// <summary>
        /// If the cell isn't in a set then add it to one, otherwise add it to the existing set then add the set for updating
        /// </summary>
        /// <param name="c">The Cell you're trying to simulate</param>
        /// <returns>The guid to the set containing the Cell</returns>
        public Guid Add(Cell c)
        {
            Guid set = new Guid();

            if (c.owningPocket != Guid.Empty)
            {
                set = c.owningPocket;
                //if (!pocketsToUpdate.Contains(set))
                //    pocketsToUpdate.AddLast(set);    
                pocketsToUpdate.Add(set);
            }
            else
            {
                set = Guid.NewGuid();
                gasPockets.Add(set, new HashSet<Cell>());
                visitedGasPockets.Add(set, new HashSet<Cell>());
                //unvisitedGasPocketCells.Add(set, new HashSet<Cell>());
                //unvisitedGasPocketCells.Add(set, new LinkedList<Cell>());
                unvisitedGasPocketCells.Add(set, new Queue<Cell>());

                c.owningPocket = set;
                gasPockets[set].Add(c);

                // This is a major slowdown
                //if (!pocketsToUpdate.Contains(set))
                //    pocketsToUpdate.AddLast(set);
                pocketsToUpdate.Add(set);
            }

            return set;
        }

        /// <summary>
        /// iterate through the cells and add their neighbours to the current pocket, it is assumed that the pocket exists already
        /// </summary>
        /// <param name="set">The gas pocket that is to be expanded</param>
        public void ExpandPocket(Guid set)
        {
            if (visitedGasPockets[set].Count == gasPockets[set].Count)
            {
                //pocketsToUpdate.AddLast(set);
                return;
            }

            HashSet<Cell> currentPocket = gasPockets[set];
            //HashSet<Cell> visitedCells;
            //HashSet<Cell> unvisitedCells;
            //LinkedList<Cell> unvisitedCells;
            Queue<Cell> unvisitedCells;
            int attempts = 0;
            
            /*
            if (visitedGasPockets[set].Count <= 0)
            {
                visitedCells = new HashSet<Cell>();
            }
            else
            {
                visitedCells = visitedGasPockets[set];
            }
            */
            if (unvisitedGasPocketCells[set].Count <= 0)
            {
                //unvisitedCells = new HashSet<Cell>();
                //unvisitedCells = new LinkedList<Cell>();
                unvisitedCells = new Queue<Cell>();
            }
            else
            {
                unvisitedCells = unvisitedGasPocketCells[set];
            }

            //unvisitedCells.AddRange(gasPockets[set].Except(unvisitedCells));
            //unvisitedCells = new Queue<Cell>(unvisitedCells.Union(gasPockets[set].Except(unvisitedCells)));
            JoinToQueue(unvisitedCells, unvisitedCells.Union(gasPockets[set].Except(unvisitedCells)));
            

            Cell c = null;
            //while ((attempts < MAX_NUMBER_TO_PROCESS) && (visitedCells.Count() != gasPockets[set].Count()))
            while ((attempts < MAX_NUMBER_TO_PROCESS) && (unvisitedCells.Count() > 0))
            {
                // TODO: this is a slowdown
                //c = unvisitedCells.First();
                c = unvisitedCells.Dequeue();

                foreach (Cell n in c.neighbours)
                {
                    // Tests will need to be added here if I add liquids

                    if (n.owningPocket == Guid.Empty)
                    {
                        n.owningPocket = set;
                        currentPocket.Add(n);
                    }
                    else if (n.owningPocket == set)
                    {
                        // This is already part of the set
                    }
                    else
                    {
                        //unvisitedCells = unvisitedCells.Union(unvisitedGasPocketCells[n.owningPocket]).ToList();
                        //unvisitedCells.AddRange(unvisitedGasPocketCells[n.owningPocket]);
                        //unvisitedCells = new Queue<Cell>(unvisitedCells.Union(unvisitedGasPocketCells[n.owningPocket]));
                        JoinToQueue(unvisitedCells, unvisitedGasPocketCells[n.owningPocket]);
                        unvisitedCells.Enqueue(n);
                        attempts += joinGasPockets(set, n.owningPocket);
                    }
                }
                //unvisitedCells.Remove(c);
                //unvisitedCells.RemoveFirst();
                
                attempts++;
            }

            if (unvisitedCells.Count != 0)
                pocketsToUpdate.Add(set);
            else
                pocketsToUpdate.Remove(set);

            unvisitedGasPocketCells[set] = unvisitedCells;
            gasPockets[set] = currentPocket;
        }

        public void JoinToQueue(Queue<Cell> lhs, IEnumerable<Cell> rhs)
        {
            foreach (Cell c in rhs)
            {
                lhs.Enqueue(c);
            }
        }

        /// <summary>
        /// Joins two gas pockets
        /// </summary>
        /// <param name="lhs">The GUID of the left hashset</param>
        /// <param name="rhs">The GUID of the left hashset</param>
        /// <returns>the number of items added</returns>
        public int joinGasPockets(Guid lhs, Guid rhs)
        {
            int toReturn = gasPockets[rhs].Count;

            foreach (Cell c in gasPockets[rhs])
            {
                c.owningPocket = lhs;
            }

            // Requires logic here to deal with any objects inside rhs that are operating on the pocket

            gasPockets[lhs].UnionWith(gasPockets[rhs]);
            gasPockets.Remove(rhs);
            pocketsToUpdate.Remove(rhs);

            return toReturn;
        }

        /// <summary>
        /// Update gas pockets and equalize pressure
        /// </summary>
        /// <returns>
        /// Returns true if pockets processed correctly
        /// </returns>
        public bool ProcessGasPocket(Guid set)
        {
            HashSet<Cell> currentSet = gasPockets[set];

            Dictionary<Gas.GasType, double> gasTotals = new Dictionary<Gas.GasType, double>();
            for (int i = 0; i < Gas.numberOfGasses; i++)
            {
                gasTotals.Add((Gas.GasType)i, 0.0d);
            }

            foreach (Cell c in currentSet)
            {
                foreach (Gas.GasType g in c.gasses.Keys)
                {
                    gasTotals[g] += c.gasses[g].pressure;
                }
            }

            foreach (Gas.GasType g in gasTotals.Keys.ToList())
            {
                if (gasTotals[g] == 0.0d)
                    gasTotals.Remove(g);
                else
                    gasTotals[g] = gasTotals[g] / (double)currentSet.Count();
            }

            foreach (Cell c in currentSet)
            {
                foreach (Gas.GasType g in c.gasses.Keys)
                {
                    if (!c.gasses.Keys.Contains(g))
                        c.gasses.Add(g, new Gas() { type = g, pressure = gasTotals[g] });
                    else
                        c.gasses[g].pressure = gasTotals[g];
                }
            }

            return true;
        }

        public void Process()
        {
            Guid set = Guid.Empty;

            if (pocketsToUpdate.Count > 0)
            {
                set = pocketsToUpdate.First();
            }
            else
                return;

            ExpandPocket(set);
            ProcessGasPocket(set);
        }
    }
}
