using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSim
{
    class Program
    {
        public static Cell[,,] world;

        static void Main(string[] args)
        {
            Random r = new Random(0);
            DateTime currentTime;

            Cell voidCell = new Cell(){gasses = new Dictionary<Gas.GasType,Gas>(), neighbours = new List<Cell>()};

            int numberOfCells = 0;
            world = new Cell[5, 5, 5];

            for (int z = 0; z < world.GetLength(0); z++)
            {
                for (int y = 0; y < world.GetLength(1); y++)
                {
                    for (int x = 0; x < world.GetLength(2); x++)
                    {
                        world[z,y,x] = new Cell();
                        //world[z, y, x].gasses.Add(Gas.GasType.AIR, new Gas() { pressure = r.NextDouble(), type = Gas.GasType.AIR });
                        //world[z, y, x].gasses.Add(Gas.GasType.KEROSENE, new Gas() { pressure = r.NextDouble(), type = Gas.GasType.KEROSENE });

                        numberOfCells++;
                    }
                }
            }

            for (int z = 0; z < world.GetLength(0); z++)
            {
                for (int y = 0; y < world.GetLength(1); y++)
                {
                    for (int x = 0; x < world.GetLength(2); x++)
                    {
                        if (z > 0) world[z, y, x].neighbours.Add(world[z - 1, y, x]); 
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }
                        if (z < world.GetLength(0)-1) world[z, y, x].neighbours.Add(world[z + 1, y, x]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }

                        if (y > 0) world[z, y, x].neighbours.Add(world[z, y - 1, x]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }
                        if (y < world.GetLength(0) - 1) world[z, y, x].neighbours.Add(world[z, y + 1, x]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }

                        if (x > 0) world[z, y, x].neighbours.Add(world[z, y, x - 1]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }
                        if (x < world.GetLength(0) - 1) world[z, y, x].neighbours.Add(world[z, y, x + 1]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }
                    }
                }
            }

            Console.WriteLine("Number of Cells: " + numberOfCells);
            world[0, 0, 0].gasses.Add(Gas.GasType.AIR, new Gas() { pressure = 12000.0, type = Gas.GasType.AIR });            

            int frames = 0;
            FluidManager fMan = new FluidManager() { voidCell = voidCell };
            /*
            foreach (Cell c in world)
            {
                fMan.Add(c);
            }
            */

            fMan.Add(world[0,0,0]);
            currentTime = DateTime.Now;

            
            Console.WriteLine("Pressure at (0,0,0) frame: " + world[0, 0, 0].gasses[Gas.GasType.AIR].pressure);

            //Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1)].gasses[Gas.GasType.AIR].pressure);
            //Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1)].gasses[Gas.GasType.AIR].pressure);
            //Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine();

            while (fMan.ProcessNodes())
            {
                frames++;
            }
            double timeTaken = (DateTime.Now - currentTime).TotalSeconds;
            Console.WriteLine();
            Console.WriteLine("Frames taken to process all cells to equilbrium: " + frames);
            Console.WriteLine("Time taken to process all cells to equilbrium: " + timeTaken);
            Console.WriteLine("Seconds per frame: " + (timeTaken/(double)frames));
            Console.WriteLine();
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(0) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.In.ReadLine();
        }
    }
}

