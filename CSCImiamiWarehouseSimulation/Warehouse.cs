///////////////////////////////////////////////////////////////////////////////
//
// Author: Daniel Foister, Curtis Reece, Sterling McClanahan, Chris Oaks
// Course: CSCI-2210-001 - Data Structures
// Assignment: Project3 - Warehouse Simulation
// Description: A demonstration of our understanding and proficiency in the data structures we have discussed in class,
//              in the form of a warehouse simulation.
//
///////////////////////////////////////////////////////////////////////////////
using CSCImiamiWarehouseSimulation;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;

namespace CSCImiamiWarehouseSimulation
{
    internal class Warehouse
    {
        public List<Dock> docks = new List<Dock>();

        public Queue<Truck> entrance = new Queue<Truck>();

        public List<Truck> allTrucks = new List<Truck>();

        public List<int> allTruckIds = new List<int>();

        public List<Crate> allDeliveredCrates = new List<Crate>();

        public List<Truck> allProcessedTrucks = new List<Truck>();

        

        public double dockCost { get; set; } = 100;
        public int timeIncrements { get; set; } = 48;

       
        ///////////////////////////////////////////////////////////
       
        List<Truck>[] trucks = new List<Truck>[48];

        
        //////////////////////////////////////////////////////////
        
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
            ClearCSV();
            GenerateTrucks(warehouse);

            //For loop that runs the actual simulation and updates every time increment.
            for (warehouse.currentTime = 0; warehouse.currentTime < warehouse.timeIncrements; warehouse.currentTime++)
            {
                //Process the docks by unloading a crate.
                //Handle the scenario of swapping to the next queued truck if last crate was unloaded.
                AssignTrucksToDocks(warehouse);

                foreach (Dock dock in warehouse.docks)
                {
                    ProcessDock(warehouse, dock);
                }
            }
        }

        static void ProcessDock(Warehouse warehouse, Dock dock)
        {
            Truck currentTruck;
            Crate currentCrate;
            Crate lastDeliveredCrate = null;
            
            if (warehouse.allDeliveredCrates.Count() > 0)
            {
                lastDeliveredCrate = warehouse.allDeliveredCrates.Last();
            }

            if (dock.Line.Count > 0)
            {
                currentTruck = dock.Line.Peek();
                if (!warehouse.allTrucks.Contains(currentTruck))
                {
                    warehouse.allTrucks.Add(currentTruck);
                    warehouse.allTruckIds.Add(currentTruck.id);
                }

                if (currentTruck.HasMoreCrates())
                {
                    currentCrate = currentTruck.Unload();
                    currentCrate.timeIncrementDelivered = warehouse.currentTime;
                    dock.TotalCrates++;
                    dock.TotalSales += currentCrate.Price;
                    currentTruck.truckWorth += currentCrate.Price;
                    warehouse.allDeliveredCrates.Add(currentCrate);

                    if (currentTruck.HasMoreCrates())
                    {
                        currentCrate.scenario = "HasMoreCrates";
                    }
                    else if (!currentTruck.HasMoreCrates() && dock.Line.Count() > 1)
                    { 
                        warehouse.allProcessedTrucks.Add(currentTruck);
                        currentCrate.scenario = "WaitingForNextTruck";
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
    
        static void GenerateTrucks(Warehouse warehouse)
        {
            for (int i = 0; i < warehouse.numberOfDocks; i++)
            {
                Dock dock = new Dock();
                warehouse.docks.Add(dock);
            }

            for (int i = 0; i < 48; i++)
            {
                warehouse.trucks[i] = new List<Truck>();
            }

            for (int i = 0; i < warehouse.timeIncrements; i++)
            {
                //This is where we need to do the normal distribution code. <--Might need this

                if (i <= warehouse.timeIncrements / 2)
                {
                    warehouse.chanceOfGeneratingTruck = i / warehouse.timeIncrements;
                }
                else
                {
                    warehouse.chanceOfGeneratingTruck = (warehouse.timeIncrements - i) / (warehouse.timeIncrements / 2);
                }

                //Attempt to make a truck a number of times equal to the max number of trucks possible per time increment.
                for (int j = 0; j < warehouse.maxPossibleTrucksPerTimeIncrement; j++)
                {
                    if (new Random().NextDouble() >= warehouse.chanceOfGeneratingTruck)
                    {
                        //Generate a truck.
                        Truck truck = Truck.GenerateTruck();
                        warehouse.trucks[i].Add(truck);
                        warehouse.numberOfTrucks++;
                    }
                }
            }
        }

        static void AssignTrucksToDocks(Warehouse warehouse)
        {
            //Truck Arrivals at the Entrance
            foreach (Truck truck in warehouse.trucks[warehouse.currentTime])
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

            if (new FileInfo(filePath).Length == 0)
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

        /// <summary>
        /// Creates a report for the Warehouse 
        /// </summary>
        /// <param name="warehouse">the warehouse being reported</param>
        public static void GenerateReport(Warehouse warehouse)
        {
            Console.WriteLine("REPORT: ");
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
            Console.WriteLine();

            Console.WriteLine("All Dock Reports: ");
            foreach (Dock dock in warehouse.docks)
            {
                Console.WriteLine("  Dock: " + dock.Id);
                Console.WriteLine("    Total Trucks Processed: " + dock.numberOfTrucksEmptied);
                Console.WriteLine("    Total Crates Processed: " + dock.TotalCrates);
                Console.WriteLine("    Total Time Used: " + dock.TimeInUse);
                Console.WriteLine("    Total Time Not Used: " + dock.TimeNotInUse);
                Console.WriteLine("    Total Sales: " + dock.TotalSales);
            }

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

