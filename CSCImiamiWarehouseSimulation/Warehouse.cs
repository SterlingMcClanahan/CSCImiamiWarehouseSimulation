///////////////////////////////////////////////////////////////////////////////
//
// Author: Daniel Foister, Curtis Reece, Sterling McClanahan, Chris Oaks
// Course: CSCI-2210-001 - Data Structures
// Assignment: Project3 - Warehouse Simulation
// Description: A demonstration of our understanding and proficiency in the data structures we have discussed in class,
//              in the form of a warehouse simulation.
//
///////////////////////////////////////////////////////////////////////////////
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace CSCImiamiWarehouseSimulation
{
    internal class Warehouse
    {

        List<Dock> docks = new List<Dock>();

        Queue<Truck> entrance = new Queue<Truck>();

        List<Truck> allTrucks = new List<Truck>();
        List<int> allTruckIds = new List<int>();


        List<Crate> allDeliveredCrates = new List<Crate>();
        

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
        //public string scenario = null;
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
            NormalDistribution truckArrivalDistribution = new NormalDistribution(); //Updated based on office hours w/ Gillenwater

            //For the number of Time Increments.
            for (int i = 0; i < warehouse.timeIncrements; i++)
            {
                //Determine how many trucks per time increment using Normal Distribution.
                int trucksThisIncrement = (int)Math.Round(truckArrivalDistribution.SampleRev2(new Random())); //Updated based on office hours w/ Gillenwater

                //Makes sure the number of trucks does not exceed the maximum possible 
                trucksThisIncrement = Math.Min(trucksThisIncrement, warehouse.maxPossibleTrucksPerTimeIncrement); //Still need?

                //This is where we need to do the normal distribution code. <--Might need this
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
                    Crate currentCrate;
                    Crate lastDeliveredCrate;
                    
                    if (dock.Line.Count > 0)
                    {
                        currentTruck = dock.Line.Peek();
                        if(!warehouse.allTrucks.Contains(currentTruck))
                        {
                            warehouse.allTrucks.Add(currentTruck);
                            warehouse.allTruckIds.Add(currentTruck.id);
                        }
                       
                        if(currentTruck.HasMoreCrates())
                        {
                            
                            currentCrate = currentTruck.Unload();
                            currentCrate.timeIncrementDelivered = warehouse.currentTime;
                            dock.TotalCrates++;
                            dock.TotalSales += currentCrate.Price;
                            currentTruck.truckWorth += currentCrate.Price;
                            warehouse.allDeliveredCrates.Add(currentCrate);
                        }
                        if (warehouse.allDeliveredCrates.Count() > 0 && currentTruck.Trailer.Count() == 0)
                        {
                            lastDeliveredCrate = warehouse.allDeliveredCrates.Last();
                            if (currentTruck.HasMoreCrates())
                            {

                                //Situation where crate has been unloaded and there are more crates to unload.
                                lastDeliveredCrate.scenario = "HasMoreCrates";
                            }
                            else
                            {
                                //Situation where crate has been unloaded and the truck has no more crates to unload.
                                dock.SendOff();
                                dock.TotalTrucks++;

                                if (dock.Line.Count > 0)
                                {
                                    //And another truck is already in the Dock
                                    lastDeliveredCrate.scenario = "WaitingForNextTruck";
                                }
                                else if (dock.Line.Count == 0)
                                {
                                    //But another truck is NOT already in the Dock
                                    lastDeliveredCrate.scenario = "NoNextTruck";
                                }
                            }
                        }
                        // I was having trouble referencing the variable of the same crate

                        //if (currentTruck.HasMoreCrates())
                        //{

                        //    //Situation where crate has been unloaded and there are more crates to unload.
                        //    //Do nothing currently, but eventually add logging info here and nothing else.
                        //    //warehouse.scenario = "HasMoreCrates";
                        //    lastDeliveredCrate.scenario = "HasMoreCrates";
                        //}
                        //else
                        //{
                        //    //Situation where crate has been unloaded and the truck has no more crates to unload.
                        //    dock.SendOff();
                        //    dock.TotalTrucks++;

                        //    if (dock.Line.Count > 0)
                        //    {
                        //        //And another truck is already in the Dock
                        //        //Do nothing currently, but eventually add logging info here and nothing else.
                        //        //warehouse.scenario = "WaitingForNextTruck";
                        //        currentCrate.scenario = "WaitingForNextTruck";
                        //    }
                        //    else if (dock.Line.Count == 0)
                        //    {
                        //        //But another truck is NOT already in the Dock
                        //        //Do nothing currently, but eventually add logging info here and nothing else.
                        //        //warehouse.scenario = "NoNextTruck";
                        //        currentCrate.scenario = "NoNextTruck";
                        //    }
                        //}

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
            Console.WriteLine("Number of Truck Ids: " + warehouse.allTruckIds.Count());

            foreach (Dock dock in warehouse.docks)
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

            Console.WriteLine("Total Crates Processed: " + warehouse.allDeliveredCrates.Count());

            warehouse.avgValueOfCrates = warehouse.allDockSales / warehouse.totalCratesProcessed;
            Console.WriteLine("Average Value of All Crates: " + warehouse.avgValueOfCrates);

            warehouse.avgDockTimeUse = warehouse.totalUsedDockTime / warehouse.numberOfDocks;
            Console.WriteLine("Average Time Each Dock Was in Use: " + warehouse.avgDockTimeUse);

            warehouse.totalCostOfOperatingEachDock = warehouse.dockCost * warehouse.numberOfDocks * warehouse.timeIncrements;
            Console.WriteLine("Total Cost of Operating Each Dock: " + warehouse.totalCostOfOperatingEachDock);

            warehouse.revenue = warehouse.allDockSales - warehouse.totalCostOfOperatingEachDock;
            Console.WriteLine("Total Revenue: " + warehouse.revenue);

            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
            }
            Console.WriteLine("Average Truck Value: " + warehouse.totalTruckValue / warehouse.allTrucks.Count);

            Console.WriteLine();

            Console.WriteLine("Each Docks Time & Sales: ");
            foreach (Dock dock in warehouse.docks)
            {
                Console.WriteLine("  Dock " + dock.Id +":");
                Console.WriteLine("    Time in use: " + dock.TimeInUse);
                Console.WriteLine("    Time not in use: " + dock.TimeNotInUse);
                Console.WriteLine("    Sales: " + dock.TotalSales);
                Console.WriteLine("    Line Length: " + dock.lineLength);
            }

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Delivered Crates: ");
            foreach (Crate crate in warehouse.allDeliveredCrates)
            {
               
                // crates id number
                Console.Write(crate.Id + ", ");
               
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("///////////////////////////////////////////////////////////////////////////");
            Console.WriteLine();
            // need to print this stuff to a csv file

            Console.WriteLine("Crate Info to CSV File:");
            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
                foreach (Crate crate in truck.deliveredCrates)
                {
                    //////////////////////////////////
                    // log each crate to a csv file //
                    //////////////////////////////////

                    //time increment crate was unloaded
                    Console.Write(crate.timeIncrementDelivered + ", ");
                    // truck drivers name
                    Console.Write("" + truck.driver + ", ");
                    // delivery companies name
                    Console.Write("" + truck.deliveryCompany + ", ");
                    // crates id number
                    Console.Write("TruckId: " + truck.id + ", ");
                    Console.Write("" + crate.Id + ", ");
                    // crates value 
                    Console.Write("" + crate.Price + ", ");
                    // string status after crate is unloaded
                    Console.WriteLine("" + crate.scenario);


                    //warehouse.LogToCSV(crate.timeIncrementDelivered, truck.driver, truck.deliveryCompany, crate, crate.scenario);
                    warehouse.LogToCSV(crate.timeIncrementDelivered, truck.driver, truck.deliveryCompany, crate.Id, crate.Price, crate.scenario);
                    //hel

                }
            }


        }
        private void LogToCSV(int timeIncrement, string driver, string company, string id, double price,string scenario)
        {
            //Does not clear the previous CSV file before making a new one

            // Replace "yourfile.csv" with the actual path and filename you want to use
            string filePath = "yourfile.csv";

            // Check if the file exists; if not, create it and write the header
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Time Increment,Driver,Delivery Company,Crate ID,Crate Value,Scenario");
                }
            }

            // Append the new log entry
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                //writer.WriteLine($"{timeIncrement},{driver},{company},{crate?.Id ?? "N/A"},{crate?.Price ?? 0},{scenario}");
                writer.WriteLine($"{timeIncrement},{driver},{company},{id},{price},{scenario}");
            }
        }
    }

}
