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
            Random r = new Random();
            DateTime currentTime;

            Cell voidCell = new Cell(){gasses = new Dictionary<Gas.GasType,Gas>(), neighbours = new List<Cell>()};

            int numberOfCells = 0;
            world = new Cell[20, 20, 20];

            for (int z = 0; z < world.GetLength(0); z++)
            {
                for (int y = 0; y < world.GetLength(1); y++)
                {
                    for (int x = 0; x < world.GetLength(2); x++)
                    {
                        world[z,y,x] = new Cell();
                        world[z, y, x].gasses.Add(Gas.GasType.AIR, new Gas() { pressure = r.NextDouble(), type = Gas.GasType.AIR });
                        world[z, y, x].gasses.Add(Gas.GasType.KEROSENE, new Gas() { pressure = r.NextDouble(), type = Gas.GasType.KEROSENE });

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
                        if (y < world.GetLength(1) - 1) world[z, y, x].neighbours.Add(world[z, y + 1, x]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }

                        if (x > 0) world[z, y, x].neighbours.Add(world[z, y, x - 1]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }
                        if (x < world.GetLength(2) - 1) world[z, y, x].neighbours.Add(world[z, y, x + 1]);
                            //else { if (!world[z, y, x].neighbours.Contains(voidCell)) world[z, y, x].neighbours.Add(voidCell); }
                    }
                }
            }

            Console.WriteLine("Number of Cells: " + numberOfCells);
            //world[0, 0, 0].gasses.Add(Gas.GasType.AIR, new Gas() { pressure = numberOfCells*1.5d, type = Gas.GasType.AIR });            

            int frames = 0;
            FluidManager fMan = new FluidManager() { voidCell = voidCell };

            Console.WriteLine("Adding Cells");
            
            foreach (Cell c in world)
            {
                fMan.Add(c);
            }
            
            Console.WriteLine("Done Adding Cells");

            Console.WriteLine("Processing");
            fMan.Add(world[0,0,0]);


            currentTime = DateTime.Now;
            while (fMan.Process() > 0)
            {
                frames++;
            }
            double timeTaken = (DateTime.Now - currentTime).TotalSeconds;

            Console.WriteLine("Pressure at (0,0,0) frame: " + world[0, 0, 0].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Frames taken to process all cells to equilbrium: " + frames);
            Console.WriteLine("Time taken to process all cells to equilbrium: " + timeTaken);
            Console.WriteLine("Seconds per frame: " + (timeTaken/(double)frames));
            Console.WriteLine("Seconds at 60FPS: " + (frames / 60.0d));
            Console.WriteLine();
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Adding more gasses");

            world[0, 0, 0].gasses[Gas.GasType.AIR].pressure += numberOfCells;
            //world[0, 0, 0].gasses[Gas.GasType.KEROSENE].pressure += numberOfCells;

            Console.WriteLine("Adding (0, 0, 0)");
            fMan.Add(world[0, 0, 0]);

            frames = 0;
            Console.WriteLine("Processing again");
            currentTime = DateTime.Now;
            while (fMan.Process() > 0)
            {
                frames++;
            }

            timeTaken = (DateTime.Now - currentTime).TotalSeconds;
            Console.WriteLine();
            Console.WriteLine("Frames taken to process all cells to equilbrium: " + frames);
            Console.WriteLine("Time taken to process all cells to equilbrium: " + timeTaken);
            Console.WriteLine("Seconds per frame: " + (timeTaken / (double)frames));
            Console.WriteLine("Seconds at 60FPS: " + (frames / 60.0d));
            Console.WriteLine();
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Adding more gasses");

            world[0, 0, 0].gasses[Gas.GasType.AIR].pressure += numberOfCells;

            Console.WriteLine("Adding (0, 0, 0)");
            fMan.Add(world[0, 0, 0]);
            frames = 0;
            Console.WriteLine("Processing again");
            currentTime = DateTime.Now;
            while (fMan.Process() > 0)
            {
                frames++;
            }

            timeTaken = (DateTime.Now - currentTime).TotalSeconds;
            Console.WriteLine();
            Console.WriteLine("Frames taken to process all cells to equilbrium: " + frames);
            Console.WriteLine("Time taken to process all cells to equilbrium: " + timeTaken);
            Console.WriteLine("Seconds per frame: " + (timeTaken / (double)frames));
            Console.WriteLine("Seconds at 60FPS: " + (frames / 60.0d));
            Console.WriteLine();
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);



            
            frames = 0;
            Console.WriteLine("Processing again");
            currentTime = DateTime.Now;
            while (fMan.Process() > 0)
            {
                frames++;
            }

            timeTaken = (DateTime.Now - currentTime).TotalSeconds;
            Console.WriteLine();
            Console.WriteLine("Frames taken to process all cells to equilbrium: " + frames);
            Console.WriteLine("Time taken to process all cells to equilbrium: " + timeTaken);
            Console.WriteLine("Seconds per frame: " + (timeTaken / (double)frames));
            Console.WriteLine("Seconds at 60FPS: " + (frames / 60.0d));
            Console.WriteLine();
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);
            Console.WriteLine("Pressure at random frame: " + world[r.Next(world.GetLength(0) - 1), r.Next(world.GetLength(1) - 1), r.Next(world.GetLength(2) - 1)].gasses[Gas.GasType.AIR].pressure);

            Console.In.ReadLine();
        }
    }
}


