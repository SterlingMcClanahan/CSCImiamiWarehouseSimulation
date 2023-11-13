///////////////////////////////////////////////////////////////////////////////
//
// Author: Daniel Foister, Curtis Reece, Sterling McClanahan, Chris Oaks
// Course: CSCI-2210-001 - Data Structures
// Assignment: Project3 - Warehouse Simulation
// Description: A demonstration of our understanding and proficiency in the data structures we have discussed in class,
//              in the form of a warehouse simulation.
//
///////////////////////////////////////////////////////////////////////////////
using System.Security.Cryptography.X509Certificates;

namespace CSCImiamiWarehouseSimulation
{
    internal class Warehouse
    {

        List<Dock> docks = new List<Dock>();
        Queue<Truck> entrance = new Queue<Truck>();

        List<Truck> allTrucks = new List<Truck>();

        

        public double dockCost { get; set; } = 100;
        public int timeIncrements { get; set; } = 48;
        public int currentTime { get; set; } = 0;
        public int numberOfDocks { get; set; } = 10;
        public int numberOfTrucks { get; set; } = 0;
        public float chanceOfGeneratingTruck { get; set; } = 0;
        public int maxPossibleTrucksPerTimeIncrement { get; set; } = 5;

        public double allDockSales = 0;

        public int longestLine = 0;

        public int totalUsedDockTime;
        public int totalUnusedDockTime;
        public int totalProcessedTrucks;
        public int totalCratesProcessed;
        public double avgDockTimeUse;
        public double avgValueOfCrates;
        public double totalCostOfOperatingEachDock;
        public double revenue;
        public double totalTruckValue;
        public Warehouse()
        {
            docks.Clear();
            entrance.Clear();
        }

        public static void Run(Warehouse warehouse)
        {
            //Initial Setup of Simulation and its parameters.
            // might need to set parameters back to starting point here

            //Setup of Docks
            for (int i = 0; i < warehouse.numberOfDocks; i++)
            {
                Dock dock = new Dock();
                warehouse.docks.Add(dock);
            }

            //Setup of Trucks
            List<Truck>[] trucks = new List<Truck>[warehouse.timeIncrements];

            for (int i = 0; i < warehouse.timeIncrements; i++)
            {
                trucks[i] = new List<Truck>();
            }

            //Creates the normal distribution for the truck arrival
            NormalDistribution truckArrivalDistribution = new NormalDistribution(warehouse.timeIncrements / 2, 5); //<--Needs to be adjusted <--!!!NEW CODE!!!

            //For the number of Time Increments.
            for(int i = 0; i < warehouse.timeIncrements; i++)
            {
                //Determine how many trucks per time increment using Normal Distribution.
                int trucksThisIncrement = (int)Math.Round(truckArrivalDistribution.Sample(new Random())); //<--!!!NEW CODE!!!

                //Makes sure the number of trucks does not exceed the maximum possible 
                trucksThisIncrement = Math.Min(trucksThisIncrement, warehouse.maxPossibleTrucksPerTimeIncrement); //<--!!!NEW CODE!!!

                //This is where we need to do the normal distribution code.
                if(i < warehouse.timeIncrements / 2)
                {
                    warehouse.chanceOfGeneratingTruck = i / warehouse.timeIncrements;
                }
                else
                {
                    warehouse.chanceOfGeneratingTruck = (warehouse.timeIncrements - i) / (warehouse.timeIncrements / 2);
                }

                //Attempt to make a truck a number of times equal to the max number of trucks possible per time increment.
                for(int j = 0; j < warehouse.maxPossibleTrucksPerTimeIncrement; j++)
                {
                    if (new Random().NextDouble() >= warehouse.chanceOfGeneratingTruck)
                    {
                        //Generate a truck.
                        Truck truck = Truck.GenerateTruck();
                        trucks[i].Add(truck);
                        warehouse.numberOfTrucks++;
                    }
                }
            }

            //For loop that runs the actual simulation and updates every time increment.
            for (warehouse.currentTime = 0; warehouse.currentTime < warehouse.timeIncrements; warehouse.currentTime++)
            {
                //Truck Arrivals at the Entrance
                foreach (Truck truck in trucks[warehouse.currentTime])
                {
                    warehouse.entrance.Enqueue(truck);
                }

                //Trucks Assigned to Docks
                foreach (Truck truck in warehouse.entrance)
                {
                    int indexOfDockWithSmallestLine = 0;
                    //Loop through each dock to find the one with the smallest line.
                    for (int j = 1; j < warehouse.docks.Count; j++)
                    {
                        if (warehouse.docks[j].Line.Count < warehouse.docks[indexOfDockWithSmallestLine].Line.Count)
                        {
                            indexOfDockWithSmallestLine = j;
                        }
                    }

                    //Add the truck to the Dock
                    warehouse.docks[indexOfDockWithSmallestLine].JoinLine(truck);
                    //Note: Trucks can be added to a dock every time increment, but it doesn't say whether multiple trucks
                    //can be added to the same dock or not. This is assuming that they can in cases of small numbers of docks and a lot of trucks.
                }

                //Process the docks by unloading a crate.
                //Handle the scenario of swapping to the next queued truck if last crate was unloaded.
                foreach (Dock dock in warehouse.docks)
                {
                    Truck currentTruck;
                    
                    if (dock.Line.Count > 0)
                    {
                        currentTruck = dock.Line.Peek();
                        if(!warehouse.allTrucks.Contains(currentTruck))
                        {
                            warehouse.allTrucks.Add(currentTruck);
                        }
                        
                        while(currentTruck.HasMoreCrates())
                        {
                            Crate currentCrate = currentTruck.Unload();
                            currentCrate.timeIncrementDelivered = warehouse.currentTime;
                            dock.TotalCrates++;
                            dock.TotalSales += currentCrate.Price;
                            currentTruck.truckWorth += currentCrate.Price;
                        }

                        if (currentTruck.HasMoreCrates())
                        {
                            //Situation where crate has been unloaded and there are more crates to unload.
                            //Do nothing currently, but eventually add logging info here and nothing else.
                        }
                        else
                        {
                            //Situation where crate has been unloaded and the truck has no more crates to unload.
                            dock.SendOff();
                            dock.TotalTrucks++;
                            
                            if (dock.Line.Count > 0)
                            {
                                //And another truck is already in the Dock
                                //Do nothing currently, but eventually add logging info here and nothing else.
                            }
                            else if (dock.Line.Count == 0)
                            {
                                //But another truck is NOT already in the Dock
                                //Do nothing currently, but eventually add logging info here and nothing else.
                            }
                        }

                        dock.TimeInUse++;
                    }
                    else
                    {
                        dock.TimeNotInUse++;
                    }
                }


            }

            
        }

        static public void PrintEverything(Warehouse warehouse)
        {
            //  confirm that all the time has passed 
            Console.WriteLine("Total Time Increments: " + warehouse.timeIncrements);
            Console.WriteLine("Current Time: " + warehouse.currentTime);
            Console.WriteLine();

            // extra information
            Console.WriteLine("Chance of Generating Truck: " + warehouse.chanceOfGeneratingTruck);
            Console.WriteLine("Max Possible Trucks Per Time Increment: " + warehouse.maxPossibleTrucksPerTimeIncrement);
            Console.WriteLine();

            //  required for report //
            Console.WriteLine("REPORT: ");
            Console.WriteLine("Number of Docks: " + warehouse.numberOfDocks);
            Console.WriteLine("Number of Trucks: " + warehouse.numberOfTrucks);

            foreach(Dock dock in warehouse.docks)
            {
                warehouse.allDockSales += dock.TotalSales;
                if(dock.lineLength > warehouse.longestLine)
                {
                    warehouse.longestLine = dock.lineLength;
                }
                warehouse.totalUsedDockTime += dock.TimeInUse;
                warehouse.totalUnusedDockTime += dock.TimeNotInUse;
                warehouse.totalProcessedTrucks += dock.numberOfTrucksEmptied;
                warehouse.totalCratesProcessed += dock.TotalCrates;
            }
            Console.WriteLine("Total Sales From all Docks: " + warehouse.allDockSales);
            Console.WriteLine("Longest Line: " + warehouse.longestLine);
            Console.WriteLine("Total Time Used by Docks: " + warehouse.totalUsedDockTime);
            Console.WriteLine("Total Time Unused by Docks: " + warehouse.totalUnusedDockTime);
            Console.WriteLine("Toal Processed Trucks: " + warehouse.totalProcessedTrucks);
            Console.WriteLine("Total Crates Processed: " + warehouse.totalCratesProcessed);

            warehouse.avgValueOfCrates = warehouse.allDockSales / warehouse.totalCratesProcessed;
            Console.WriteLine("Average Value of All Crates: " + warehouse.avgValueOfCrates);

            warehouse.avgDockTimeUse = warehouse.totalUsedDockTime / warehouse.numberOfDocks;
            Console.WriteLine("Average Time Each Dock Was in Use: " + warehouse.avgDockTimeUse);

            warehouse.totalCostOfOperatingEachDock = warehouse.dockCost * warehouse.numberOfDocks * warehouse.timeIncrements;
            Console.WriteLine("Total Cost of Operating Each Dock: " + warehouse.totalCostOfOperatingEachDock);

            warehouse.revenue = warehouse.allDockSales - warehouse.totalCostOfOperatingEachDock;
            Console.WriteLine("Total Revenue: " + warehouse.revenue);

            Console.WriteLine();

            Console.WriteLine("Each Docks Time & Sales: ");
            foreach (Dock dock in warehouse.docks)
            {
                Console.WriteLine("  Dock " + dock.Id +":");
                Console.WriteLine("    Time in use: " + dock.TimeInUse);
                Console.WriteLine("    Time not in use: " + dock.TimeNotInUse);
                Console.WriteLine("    Sales: " + dock.TotalSales);
                Console.WriteLine("    Longest Line: " + dock.lineLength);
            }

            
            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
                foreach (Crate crate in truck.deliveredCrates)
                {
                    //////////////////////////////////
                    // log each crate to a csv file //
                    //////////////////////////////////

                    //time increment crate was unloaded
                    Console.WriteLine(crate.timeIncrementDelivered);
                    // truck drivers name
                    Console.WriteLine("  " + truck.driver);
                    // delivery companies name
                    Console.WriteLine("  " + truck.deliveryCompany);
                    // crates id number
                    Console.WriteLine("    " + crate.Id);
                    // crates value 
                    Console.WriteLine("    " + crate.Price);
                    // string status after crate is unloaded

                    //help

                }
            }

            Console.WriteLine("Average Truck Value: " + warehouse.totalTruckValue / warehouse.allTrucks.Count);

        }

    }

}
