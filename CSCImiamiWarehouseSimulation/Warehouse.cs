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
using System.Runtime.Intrinsics.X86;
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

        List<Truck> allProcessedTrucks = new List<Truck>();

        public double dockCost { get; set; } = 100;
        public int timeIncrements { get; set; } = 48;
        public int currentTime = 0;
        public int numberOfDocks { get; set; }
        public int numberOfTrucks { get; set; } = 0;
        public float chanceOfGeneratingTruck;
        public int maxPossibleTrucksPerTimeIncrement { get; set; } = 2;

        public double allDockSales { get; set; } = 0;
        public int longestLine { get; set; } = 0;
        public int totalUsedDockTime { get; set; }
        public int totalUnusedDockTime { get; set; }
        public int totalProcessedTrucks { get; set; }
        public int totalCratesProcessed { get; set; }
        public double avgDockTimeUse { get; set; }
        public double avgValueOfCrates { get; set; }
        public double totalCostOfOperatingEachDock { get; set; }
        public double revenue { get; set; }
        public double totalTruckValue { get; set; }
        public double avgValueOfTrucks { get; set; }

        public Warehouse()
        {
            docks.Clear();
            entrance.Clear();
        }

        /// <summary>
        /// Runs the simulation
        /// </summary>
        /// <param name="warehouse">The warehouse the simulation is running</param>
        public static void Run(Warehouse warehouse)
        {
            //Initial Setup of Simulation and its parameters.
            // might need to set parameters back to starting point here

            ClearCSV();

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
                //trucksThisIncrement = Math.Min(trucksThisIncrement, warehouse.maxPossibleTrucksPerTimeIncrement); //Still need?

                //This is where we need to do the normal distribution code. <--Might need this
             
                if(i <= warehouse.timeIncrements / 2)
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
                    for (int j = 1; j < warehouse.docks.Count(); j++)
                    {
                        if (warehouse.docks[j].Line.Count < warehouse.docks[indexOfDockWithSmallestLine].Line.Count())
                        {
                            indexOfDockWithSmallestLine = j;
                        }
                    }
                    
                    //Add the truck to the Dock
                    warehouse.docks[indexOfDockWithSmallestLine].JoinLine(truck);
                    //Note: Trucks can be added to a dock every time increment, but it doesn't say whether multiple trucks
                    //can be added to the same dock or not. This is assuming that they can in cases of small numbers of docks and a lot of trucks.
                }
                warehouse.entrance.Clear();

                //Process the docks by unloading a crate.
                //Handle the scenario of swapping to the next queued truck if last crate was unloaded.
                foreach (Dock dock in warehouse.docks)
                {
                    Truck currentTruck;
                    Crate currentCrate;
                    Crate lastDeliveredCrate = null;
                    if(warehouse.allDeliveredCrates.Count() > 0)
                    {
                        lastDeliveredCrate = warehouse.allDeliveredCrates.Last();
                    }

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

                            if(currentTruck.HasMoreCrates())
                            {
                                currentCrate.scenario = "HasMoreCrates";
                            }
                            else if(!currentTruck.HasMoreCrates() && dock.Line.Count() > 1)
                            {
                                
                                warehouse.allProcessedTrucks.Add(currentTruck);
                                if (dock.Line.Count > 0)
                                {
                                    currentCrate.scenario = "WaitingForNextTruck";
                                }
                                dock.SendOff();
                            }
                            else
                            {
                                currentCrate.scenario = "NoNextTruck";
                                dock.SendOff();
                                
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



        /// <summary>
        /// Prints the report as well as some other information. this is for testing purposes
        /// </summary>
        /// <param name="warehouse">the warehouse being reported</param>
        static public void PrintEverything(Warehouse warehouse)
        {
            //confirm that all the time has passed
            Console.WriteLine("Total Time Increments: " + warehouse.timeIncrements);
            Console.WriteLine("Current Time: " + warehouse.currentTime);
            Console.WriteLine();

            //extra information
            Console.WriteLine("Chance of Generating Truck: " + warehouse.chanceOfGeneratingTruck);
            Console.WriteLine("Max Possible Trucks Per Time Increment: " + warehouse.maxPossibleTrucksPerTimeIncrement);
            Console.WriteLine();

            //required for report //

            Console.WriteLine("REPORT: ");
            Console.WriteLine("Number of Docks: " + warehouse.numberOfDocks);
            Console.WriteLine("Number of Trucks: " + warehouse.numberOfTrucks);
            Console.WriteLine("Number of Truck Ids: " + warehouse.allTruckIds.Count());

            foreach (Dock dock in warehouse.docks)
            {
                warehouse.allDockSales += dock.TotalSales;
                if (dock.lineLength > warehouse.longestLine)
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

            //the dock processed truck counter is off, which means trucks are being processed wrong
            Console.WriteLine("Toal Processed Trucks by dock counter: " + warehouse.totalProcessedTrucks);
            Console.WriteLine("Total processed trucks by warehouse list: " + warehouse.allProcessedTrucks.Count());

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

            Console.WriteLine("Crate Info to CSV File:");


            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
                foreach (Crate crate in truck.deliveredCrates)
                {
                    //time increment crate was unloaded
                    Console.Write(crate.timeIncrementDelivered + ", ");
                    // truck drivers name
                    Console.Write("" + truck.driver + ", ");
                    // delivery companies name
                    Console.Write("" + truck.deliveryCompany + ", ");
                    // trucks id number
                    Console.Write("TruckId: " + truck.id + ", ");
                    // crates id number
                    Console.Write("" + crate.Id + ", ");
                    // crates value 
                    Console.Write("" + crate.Price + ", ");
                    // string status after crate is unloaded
                    Console.WriteLine("" + crate.scenario);

                    warehouse.LogToCSV(crate.timeIncrementDelivered, truck.driver, truck.deliveryCompany, crate.Id, crate.Price, crate.scenario);
                }
            }
        }

        /// <summary>
        /// Logs information into a Comma Seperated list
        /// </summary>
        /// <param name="timeIncrement">the time increment</param>
        /// <param name="driver">the truck driver</param>
        /// <param name="company">the company the truck driver works for</param>
        /// <param name="id">the ID for the crate being delivered</param>
        /// <param name="price">the price of the crate being delivered</param>
        /// <param name="scenario">Whethere the truck has more crates, has no crates and is waiting for the next truck, or has no crates and is not waiting</param>
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
                writer.WriteLine($"{timeIncrement},{driver},{company},{id},{price},{scenario}");
            }
        }

        /// <summary>
        /// Clears the previous data from the CSV so that new data can take its place.
        /// </summary>
        static void ClearCSV()
        {
            // Replace "yourfile.csv" with the actual path and filename you want to use
            using (StreamWriter writer = new StreamWriter(@"yourfile.csv", false))
            {
                
            }
        }

        public static void CalculateData(Warehouse warehouse)
        {
            foreach (Dock dock in warehouse.docks)
            {
                warehouse.allDockSales += dock.TotalSales;
                if (dock.lineLength > warehouse.longestLine)
                {
                    warehouse.longestLine = dock.lineLength;
                }
                warehouse.totalUsedDockTime += dock.TimeInUse;
                warehouse.totalUnusedDockTime += dock.TimeNotInUse;
                warehouse.totalProcessedTrucks += dock.numberOfTrucksEmptied;
                warehouse.totalCratesProcessed += dock.TotalCrates;
            }
            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
            }
            warehouse.avgValueOfCrates = Math.Round(warehouse.allDockSales / warehouse.totalCratesProcessed, 2);
            warehouse.avgValueOfTrucks = Math.Round(warehouse.totalTruckValue / warehouse.allTrucks.Count, 2);
            warehouse.avgDockTimeUse = warehouse.totalUsedDockTime / warehouse.numberOfDocks;
            warehouse.totalCostOfOperatingEachDock = warehouse.dockCost * warehouse.numberOfDocks * warehouse.timeIncrements;
            warehouse.revenue = warehouse.allDockSales - warehouse.totalCostOfOperatingEachDock;
            
        }

        public static void GenerateReport(Warehouse warehouse)
        {
            Console.WriteLine("REPORT:");
            Console.WriteLine("Number of Docks: " + warehouse.numberOfDocks);
            Console.WriteLine("Longest Line: " + warehouse.longestLine);
            Console.WriteLine("Toal Trucks Processed: " + warehouse.totalProcessedTrucks);
            Console.WriteLine("Total Crates Processed: " + warehouse.allDeliveredCrates.Count());
            Console.WriteLine("Total Sales From all Docks: " + warehouse.allDockSales);
            Console.WriteLine("Average Value of Each Crate: " + warehouse.avgValueOfCrates);
            Console.WriteLine("Average Value of Each Truck: " + warehouse.avgValueOfTrucks);
            Console.WriteLine("Total Time Used by Docks: " + warehouse.totalUsedDockTime);
            Console.WriteLine("Total Time Unused by Docks: " + warehouse.totalUnusedDockTime);
            Console.WriteLine("Average Time Each Dock Was in Use: " + warehouse.avgDockTimeUse);
            Console.WriteLine("Total Cost of Operating Each Dock: " + warehouse.totalCostOfOperatingEachDock);
            Console.WriteLine("Total Revenue: " + warehouse.revenue);

            //this goes to the csv file
            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
                foreach (Crate crate in truck.deliveredCrates)
                {
                    warehouse.LogToCSV(crate.timeIncrementDelivered, truck.driver, truck.deliveryCompany, crate.Id, crate.Price, crate.scenario);
                }
            }
        }
    }
}
