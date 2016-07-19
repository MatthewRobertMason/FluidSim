using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace FluidSim
{
    class Program
    {
        
        public static World world;

        static void Main(string[] args)
        {
            world = new World(100, 100, 100);

            Random r = new Random();
            DateTime currentTime;

            Cell voidCell = new Cell();

            int numberOfCells = 0;

            for (int z = 0; z < world.Cell.GetLength(0); z++)
            {
                for (int y = 0; y < world.Cell.GetLength(1); y++)
                {
                    for (int x = 0; x < world.Cell.GetLength(2); x++)
                    {
                        world.Cell[z,y,x] = new Cell();
                        world.Cell[z, y, x].Gasses.Add(0, r.NextDouble() );
                        world.Cell[z, y, x].Gasses.Add(1, r.NextDouble() );

                        numberOfCells++;
                    }
                }
            }

            for (int z = 0; z < world.Cell.GetLength(0); z++)
            {
                for (int y = 0; y < world.Cell.GetLength(1); y++)
                {
                    for (int x = 0; x < world.Cell.GetLength(2); x++)
                    {
                        if (z > 0) world.Cell[z, y, x].Neighbours.Add(world.Cell[z - 1, y, x]); 
                            //else { if (!world[z, y, x].Neighbours.Contains(voidCell)) world[z, y, x].Neighbours.Add(voidCell); }
                        if (z < world.Cell.GetLength(0) - 1) world.Cell[z, y, x].Neighbours.Add(world.Cell[z + 1, y, x]);
                            //else { if (!world[z, y, x].Neighbours.Contains(voidCell)) world[z, y, x].Neighbours.Add(voidCell); }

                        if (y > 0) world.Cell[z, y, x].Neighbours.Add(world.Cell[z, y - 1, x]);
                            //else { if (!world[z, y, x].Neighbours.Contains(voidCell)) world[z, y, x].Neighbours.Add(voidCell); }
                        if (y < world.Cell.GetLength(1) - 1) world.Cell[z, y, x].Neighbours.Add(world.Cell[z, y + 1, x]);
                            //else { if (!world[z, y, x].Neighbours.Contains(voidCell)) world[z, y, x].Neighbours.Add(voidCell); }

                        if (x > 0) world.Cell[z, y, x].Neighbours.Add(world.Cell[z, y, x - 1]);
                            //else { if (!world[z, y, x].Neighbours.Contains(voidCell)) world[z, y, x].Neighbours.Add(voidCell); }
                        if (x < world.Cell.GetLength(2) - 1) world.Cell[z, y, x].Neighbours.Add(world.Cell[z, y, x + 1]);
                            //else { if (!world[z, y, x].Neighbours.Contains(voidCell)) world[z, y, x].Neighbours.Add(voidCell); }
                    }
                }
            }

            Console.WriteLine("Number of Cells: " + numberOfCells);
            //world[0, 0, 0].Gasses.Add(0, new Gas() { pressure = numberOfCells*1.5d, type = 0 });            

            int frames = 0;
            FluidManager fMan = new FluidManager() { voidCell = voidCell };
            fMan.world = world;

            Console.WriteLine("Adding Cells");

            foreach (Cell c in world.Cell)
            {
                fMan.Add(c);
            }
            
            Console.WriteLine("Done Adding Cells");

            Console.WriteLine("Processing");
            fMan.Add(world.Cell[0, 0, 0]);


            currentTime = DateTime.Now;
            while (fMan.Update())
            {
                frames++;
            }
            double timeTaken = (DateTime.Now - currentTime).TotalSeconds;

            Console.WriteLine("Pressure at (0,0,0) frame: " + world.Cell[0, 0, 0].Gasses[0]);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Frames taken to process all cells to equilbrium: " + frames);
            Console.WriteLine("Time taken to process all cells to equilbrium: " + timeTaken);
            Console.WriteLine("Seconds per frame: " + (timeTaken/(double)frames));
            Console.WriteLine("Seconds at 60FPS: " + (frames / 60.0d));
            Console.WriteLine();
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Adding more Gasses");

            world.Cell[0, 0, 0].Gasses[0] += numberOfCells;
            //world[0, 0, 0].Gasses[1] += numberOfCells;

            Console.WriteLine("Adding (0, 0, 0)");
            fMan.Add(world.Cell[0, 0, 0]);

            frames = 0;
            Console.WriteLine("Processing again");
            currentTime = DateTime.Now;
            while (fMan.Update())
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
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Adding more gasses");

            world.Cell[0, 0, 0].Gasses[0] += numberOfCells;

            Console.WriteLine("Adding (0, 0, 0)");
            fMan.Add(world.Cell[0, 0, 0]);
            frames = 0;
            Console.WriteLine("Processing again");
            currentTime = DateTime.Now;
            while (fMan.Update())
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
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);



            
            frames = 0;
            Console.WriteLine("Processing again");
            currentTime = DateTime.Now;
            while (fMan.Update())
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
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);
            Console.WriteLine("Pressure at random frame: " + world.Cell[r.Next(world.Cell.GetLength(0) - 1), r.Next(world.Cell.GetLength(1) - 1), r.Next(world.Cell.GetLength(2) - 1)].Gasses[0]);

            Console.In.ReadLine();





        }
    }
}


