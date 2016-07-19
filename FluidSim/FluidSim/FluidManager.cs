using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//ing System.Threading.Tasks;

namespace FluidSim
{
    public class FluidManager
    {
        public World world;

        enum ProcessingState
        {
            WAITING,
            EXPAND,
            CACULATE_TOTALS,
            APPLYING_AVERAGE,
            CREATE_POCKET,
            SPLIT_POCKET,
            VOID_CELLS
        }

        public string[] GasTypes;

        public int MAX_NUMBER_TO_PROCESS = 5000;

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

        public Cell voidCell;

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

        /// <summary>
        /// iterate through the cells and add their neighbours to the current pocket, it is assumed that the pocket exists already
        /// </summary>
        /// <param name="set">The gas pocket that is to be expanded</param>
        public int ExpandPocket(Guid set)
        {
            if (visitedGasPockets[set].Count == gasPockets[set].Count)
            {
                return 0;
            }

            HashSet<ICell> currentPocket = gasPockets[set];
            Queue<ICell> unvisitedCells;
            int attempts = 0;

            if (unvisitedGasPocketCells[set].Count <= 0)
            {
                unvisitedCells = new Queue<ICell>();
                unvisitedCells.Enqueue(HsGetFirst(gasPockets[set]));
            }
            else
            {
                unvisitedCells = unvisitedGasPocketCells[set];
            }

            ICell c = null;
            while ((attempts < MAX_NUMBER_TO_PROCESS) && (unvisitedCells.Count > 0))
            {
                c = unvisitedCells.Dequeue();

                foreach (ICell n in c.Neighbours)
                {
                    // Tests will need to be added here if I add liquids

                    if (n.OwningPocket == Guid.Empty)
                    {
                        n.OwningPocket = set;
                        currentPocket.Add(n);
                        unvisitedCells.Enqueue(n);
                    }
                    else if (n.OwningPocket == set)
                    {
                        // This is already part of the set
                    }
                    else
                    {
                        JoinToQueue(unvisitedCells, unvisitedGasPocketCells[n.OwningPocket]);
                        unvisitedCells.Enqueue(n);
                        attempts += joinGasPockets(set, n.OwningPocket);
                    }
                }

                attempts++;
            }

            unvisitedGasPocketCells[set] = unvisitedCells;
            gasPockets[set] = currentPocket;

            return attempts;
        }

        public void JoinToQueue(Queue<ICell> lhs, IEnumerable<ICell> rhs)
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
        public int joinGasPockets(Guid lhs, Guid rhs)
        {
            int toReturn = gasPockets[rhs].Count;

            foreach (Cell c in gasPockets[rhs])
            {
                c.OwningPocket = lhs;
            }

            // Requires logic here to deal with any objects inside rhs that are operating on the pocket

            gasPockets[lhs].UnionWith(gasPockets[rhs]);
            gasPockets.Remove(rhs);
            pocketsToUpdate.Remove(rhs);

            return toReturn;
        }

        public ICell HsGetFirst(HashSet<ICell> set)
        {
            HashSet<ICell>.Enumerator e = set.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        public Guid HsGetFirst(HashSet<Guid> set)
        {
            HashSet<Guid>.Enumerator e = set.GetEnumerator();
            e.MoveNext();
            return e.Current;
        }

        public List<int> GasKeysToList(Dictionary<int, double>.KeyCollection keys)
        {
            List<int> list = new List<int>();
            foreach (int g in keys)
            {
                list.Add(g);
            }

            return list;
        }

        public Cell[] HashSetToArray(HashSet<Cell> hsc)
        {
            Cell[] cells = new Cell[hsc.Count];
            int i = 0;
            foreach (Cell c in hsc)
            {
                cells[i++] = c;
            }
            return cells;
        }

        /// <summary>
        /// Process a given gas pocket
        /// </summary>
        /// <param name="stepsSoFar">The steps so far (if any)</param>
        /// <returns>The steps taken</returns>
        public int ProcessGasPocket(int stepsSoFar)
        {
            switch (processingState)
            {
                case ProcessingState.CACULATE_TOTALS:
                    {
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
                        break;
                    }

                case ProcessingState.APPLYING_AVERAGE:
                    {
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
                                {
                                    // TODO: Update type = 1 and make sure it has the proper index of the gas array in FluidManager
                                    processingSetEnumerator.Current.Gasses.Add(g, currentGasTotals[g] );
                                }
                                else
                                    processingSetEnumerator.Current.Gasses[g] = currentGasTotals[g];
                            }
                        }

                        break;
                    }
            }
            return stepsSoFar;
        }

        /// <summary>
        /// Process Simulation Steps
        /// </summary>
        /// <returns>the number of steps performed</returns>
        public int Process()
        {
            int stepsTaken = 0;

            switch (processingState)
            {
                case ProcessingState.EXPAND:
                    {
                        if (stepsTaken < MAX_NUMBER_TO_PROCESS)
                        {
                            if ((processingSet == Guid.Empty) && (pocketsToUpdate.Count > 0))
                            {
                                processingSet = HsGetFirst(pocketsToUpdate);

                                if (processingSet == Guid.Empty)
                                    return stepsTaken;
                            }

                            stepsTaken += ExpandPocket(processingSet);
                        }

                        break;
                    }

                case ProcessingState.APPLYING_AVERAGE:
                case ProcessingState.CACULATE_TOTALS:
                    {
                        if (stepsTaken < MAX_NUMBER_TO_PROCESS)// && (set != Guid.Empty))
                        {
                            if ((processingSet.Equals(Guid.Empty)) && (pocketsToUpdate.Count > 0))
                            {
                                processingSet = HsGetFirst(pocketsToUpdate);

                                if (processingSet == Guid.Empty)
                                    return stepsTaken;
                            }

                            stepsTaken += ProcessGasPocket(stepsTaken);
                        }
                        break;
                    }
            }
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
        /// <returns>True if there is more work to do, i.e the state machine isn't currently in WAITING</returns>
        public bool Update()
        {
            int processed = Process();

            // I might move this to the ends of the individual stages
            TranisitionState(processed);

            //Console.Write("Cells Processed: " + processed);
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

            //Console.Write("Cells Processed: " + processed);
        }
        #endregion

    }
}
