using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//ing System.Threading.Tasks;

namespace FluidSim
{
    public class FluidManager
    {
        public int MAX_NUMBER_TO_PROCESS = 5000;

        public IWorld world;
        public string[] GasTypes;
        public ICell voidCell;
        
        enum ProcessingState
        {
            WAITING,
            EXPAND,
            CACULATE_TOTALS,
            APPLYING_AVERAGE,
            SPLIT_POCKET,
            VOID_CELLS
        }
        
        private HashSet<Guid> pocketsToUpdate;
        private Dictionary<Guid, HashSet<ICell>> gasPockets;

        // Used in expand gas pocket to keep track of visited cells for current and following cells
        private Dictionary<Guid, HashSet<ICell>> visitedGasPockets; 
        private Dictionary<Guid, Queue<ICell>> unvisitedGasPocketCells;
        private Dictionary<int, double> currentGasTotals;

        private Guid processingSet;
        private HashSet<ICell>.Enumerator processingSetEnumerator;
        private bool processingSetEnumeratorHasNext;
        private ProcessingState processingState;

        

        public FluidManager()
        {
            visitedGasPockets = new Dictionary<Guid, HashSet<ICell>>();
            unvisitedGasPocketCells = new Dictionary<Guid, Queue<ICell>>();
            gasPockets = new Dictionary<Guid, HashSet<ICell>>();
            pocketsToUpdate = new HashSet<Guid>();

            processingSet = Guid.Empty;
            processingSetEnumerator = new HashSet<ICell>.Enumerator();
            processingSetEnumeratorHasNext = false;
            processingState = ProcessingState.WAITING;
            currentGasTotals = new Dictionary<int, double>();


            GasTypes = new string[2];
            GasTypes[0] = "";
            GasTypes[1] = "";
        }

        /// <summary>
        /// If the cell isn't in a set then add it to one, otherwise add it to the existing set then add the set for updating
        /// </summary>
        /// <param name="c">The Cell you're trying to simulate</param>
        /// <returns>The guid to the set containing the Cell</returns>
        public Guid Add(ICell c)
        {
            Guid set = new Guid();

            if (c.OwningPocket != Guid.Empty)
            {
                set = c.OwningPocket;
                pocketsToUpdate.Add(set);
            }
            else
            {
                set = Guid.NewGuid();
                gasPockets.Add(set, new HashSet<ICell>());
                visitedGasPockets.Add(set, new HashSet<ICell>());
                unvisitedGasPocketCells.Add(set, new Queue<ICell>());

                c.OwningPocket = set;
                gasPockets[set].Add(c);

                pocketsToUpdate.Add(set);
            }

            return set;
        }

        private void JoinToQueue(Queue<ICell> lhs, IEnumerable<ICell> rhs)
        {
            foreach (ICell c in rhs)
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
        private int joinGasPockets(Guid lhs, Guid rhs)
        {
            int toReturn = gasPockets[rhs].Count;

            foreach (ICell c in gasPockets[rhs])
            {
                c.OwningPocket = lhs;
            }

            // Requires logic here to deal with any objects inside rhs that are operating on the pocket

            gasPockets[lhs].UnionWith(gasPockets[rhs]);
            gasPockets.Remove(rhs);
            pocketsToUpdate.Remove(rhs);

            return toReturn;
        }

        private ICell HsGetFirst(HashSet<ICell> set)
        {
            HashSet<ICell>.Enumerator e = set.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        private Guid HsGetFirst(HashSet<Guid> set)
        {
            HashSet<Guid>.Enumerator e = set.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        private List<int> GasKeysToList(Dictionary<int, double>.KeyCollection keys)
        {
            List<int> list = new List<int>();
            foreach (int g in keys)
            {
                list.Add(g);
            }

            return list;
        }

        private ICell[] HashSetToArray(HashSet<ICell> hsc)
        {
            ICell[] cells = new ICell[hsc.Count];
            int i = 0;
            foreach (ICell c in hsc)
            {
                cells[i++] = c;
            }
            return cells;
        }

        /// <summary>
        /// iterate through the cells and add their neighbours to the current pocket, it is assumed that the pocket exists already
        /// </summary>
        /// <param name="processingSet">The gas pocket that is to be expanded</param>
        private int ExpandPocket(int stepsSoFar)
        {
            if (stepsSoFar < MAX_NUMBER_TO_PROCESS)
            {
                if ((processingSet == Guid.Empty) && (pocketsToUpdate.Count > 0))
                {
                    processingSet = HsGetFirst(pocketsToUpdate);

                    if (processingSet == Guid.Empty)
                        return stepsSoFar;
                }

                if (visitedGasPockets[processingSet].Count == gasPockets[processingSet].Count)
                {
                    return 0;
                }

                HashSet<ICell> currentPocket = gasPockets[processingSet];
                Queue<ICell> unvisitedCells;
                
                if (unvisitedGasPocketCells[processingSet].Count <= 0)
                {
                    unvisitedCells = new Queue<ICell>();
                    unvisitedCells.Enqueue(HsGetFirst(gasPockets[processingSet]));
                }
                else
                {
                    unvisitedCells = unvisitedGasPocketCells[processingSet];
                }

                ICell c = null;
                while ((stepsSoFar < MAX_NUMBER_TO_PROCESS) && (unvisitedCells.Count > 0))
                {
                    c = unvisitedCells.Dequeue();

                    foreach (ICell n in c.Neighbours)
                    {
                        // Tests will need to be added here if I add liquids

                        if (n.OwningPocket == Guid.Empty)
                        {
                            n.OwningPocket = processingSet;
                            currentPocket.Add(n);
                            unvisitedCells.Enqueue(n);
                        }
                        else if (n.OwningPocket == processingSet)
                        {
                            // This is already part of the set
                        }
                        else
                        {
                            JoinToQueue(unvisitedCells, unvisitedGasPocketCells[n.OwningPocket]);
                            unvisitedCells.Enqueue(n);
                            stepsSoFar += joinGasPockets(processingSet, n.OwningPocket);
                        }
                    }

                    stepsSoFar++;
                }

                unvisitedGasPocketCells[processingSet] = unvisitedCells;
                gasPockets[processingSet] = currentPocket;
            }

            return stepsSoFar;
        }

        /// <summary>
        /// Caculates the average for each gas in all of the cells
        /// </summary>
        /// <param name="stepsSoFar">The ammount of steps taken thus far</param>
        /// <returns>The ammount of steps taken after this operation</returns>
        private int CaculateTotals(int stepsSoFar)
        {
            if (stepsSoFar < MAX_NUMBER_TO_PROCESS)// && (set != Guid.Empty))
            {
                if ((processingSet.Equals(Guid.Empty)) && (pocketsToUpdate.Count > 0))
                {
                    processingSet = HsGetFirst(pocketsToUpdate);

                    if (processingSet == Guid.Empty)
                        return stepsSoFar;
                }

                if (processingSetEnumeratorHasNext == false)
                {
                    processingSetEnumerator = gasPockets[processingSet].GetEnumerator();

                    currentGasTotals = new Dictionary<int, double>();
                }

                if (currentGasTotals.Count == 0)
                {
                    for (int i = 0; i < GasTypes.Count(); i++)
                    {
                        currentGasTotals.Add(i, 0.0d);
                    }
                }

                while ((stepsSoFar++ < MAX_NUMBER_TO_PROCESS) && (processingSetEnumeratorHasNext = processingSetEnumerator.MoveNext()))
                {
                    foreach (int g in processingSetEnumerator.Current.Gasses.Keys)
                    {
                        currentGasTotals[g] += processingSetEnumerator.Current.Gasses[g];
                    }
                }
            }

            return stepsSoFar;
        }

        /// <summary>
        /// Process a given gas pocket
        /// </summary>
        /// <param name="stepsSoFar">The steps so far (if any)</param>
        /// <returns>The steps taken</returns>
        private int ApplyAverage(int stepsSoFar)
        {
            if (stepsSoFar < MAX_NUMBER_TO_PROCESS)// && (set != Guid.Empty))
            {
                if ((processingSet.Equals(Guid.Empty)) && (pocketsToUpdate.Count > 0))
                {
                    processingSet = HsGetFirst(pocketsToUpdate);

                    if (processingSet == Guid.Empty)
                        return stepsSoFar;
                }

                // If we're just starting
                if (processingSetEnumeratorHasNext == false)
                {
                    processingSetEnumerator = gasPockets[processingSet].GetEnumerator();

                    // Also average all of the totals
                    foreach (int g in currentGasTotals.Keys.ToArray())
                    {
                        currentGasTotals[g] = (currentGasTotals[g] / ((double)gasPockets[processingSet].Count));
                    }
                }

                while ((stepsSoFar++ < MAX_NUMBER_TO_PROCESS) && (processingSetEnumeratorHasNext = processingSetEnumerator.MoveNext()))
                {
                    foreach (int g in currentGasTotals.Keys)
                    {
                        if (!processingSetEnumerator.Current.Gasses.ContainsKey(g))
                            processingSetEnumerator.Current.Gasses.Add(g, currentGasTotals[g]);
                        else
                            processingSetEnumerator.Current.Gasses[g] = currentGasTotals[g];
                    }
                }
            }

            return stepsSoFar;
        }

        private int SplitPocket(int stepsSoFar)
        {
            /*
             * 1)  Wall placed in pocket
             * 2)  Split attempt starts
             * 3)  Attempt to connect cell_1 with (cell_0_1, cell_0_2)
             *     a) If another wall is placed in pocket
             *         i)   Stop the current split for the pocket (previous search is invalid as the world has changed)
             *         ii)  Attempt to connect cell_x_1 with all previous cells (cell_1, cell_2, cell_11, cell_22)
             *         iii) If all cells are connected then the pocket isn't split
             *     b) If not all cells can be reached
             *         i)   The current search space becomes the current pocket and all other cells are removed from the 
             *              current pocket and added back into the manager     
             */

            return stepsSoFar;
        }

        /// <summary>
        /// Process Simulation Steps
        /// </summary>
        /// <returns>The number of steps performed</returns>
        private int Process()
        {
            int stepsTaken = 0;

            switch (processingState)
            {
                case ProcessingState.WAITING:
                    {
                        break;
                    }
                case ProcessingState.EXPAND:
                    {
                        stepsTaken += ExpandPocket(stepsTaken);
                        break;
                    }

                case ProcessingState.CACULATE_TOTALS:
                    {
                        stepsTaken += CaculateTotals(stepsTaken);
                        break;
                    }

                case ProcessingState.APPLYING_AVERAGE:
                    {
                        stepsTaken += ApplyAverage(stepsTaken);
                        break;
                    }

                case ProcessingState.SPLIT_POCKET:
                    {
                        break;
                    }

                case ProcessingState.VOID_CELLS:
                    {
                        break;
                    }

                default:
                    processingState = ProcessingState.WAITING;
                    break;
            }

            TranisitionState(stepsTaken);
            return stepsTaken;
        }

        /// <summary>
        /// Change the processing state if applicable
        /// </summary>
        /// <param name="processed">The number of processed steps last time Process() was called</param>
        private void TranisitionState(int processed)
        {
            switch (processingState)
            {
                case ProcessingState.WAITING:
                    {
                        processingState = ProcessingState.EXPAND;
                        break;
                    }

                case ProcessingState.EXPAND:
                    {
                        if (processed < MAX_NUMBER_TO_PROCESS)
                            processingState = ProcessingState.CACULATE_TOTALS;
                        break;
                    }

                case ProcessingState.CACULATE_TOTALS:
                    {
                        if (processed <= MAX_NUMBER_TO_PROCESS)
                            processingState = ProcessingState.APPLYING_AVERAGE;
                        break;
                    }

                case ProcessingState.APPLYING_AVERAGE:
                    {
                        if (processed <= MAX_NUMBER_TO_PROCESS)
                            processingState = ProcessingState.WAITING;
                        break;
                    }
            }
        }

        /// <summary>
        /// Update is called once per frame, useful for testing frame counts or if you have a special game loop
        /// </summary>
        /// <returns>True if there is more work to do, i.e the state machine isn't currently in the WAITING state</returns>
        public bool Update()
        {
            int processed = Process();

            if (processingState == ProcessingState.WAITING)
                return false;
            else
                return true;
        }

#region Unity/Other Engine Functions
        
        /// <summary>
        /// Use this for initialization, use this method is for use in Unity, or other game engines
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// Update is called once per frame, use this method is for use in Unity, or other game engines probably
        /// </summary>
        public void Update(int a)
        {
            int processed = Process();

            // I might move this to the ends of the individual stages
            TranisitionState(processed);
        }
#endregion

    }
}
