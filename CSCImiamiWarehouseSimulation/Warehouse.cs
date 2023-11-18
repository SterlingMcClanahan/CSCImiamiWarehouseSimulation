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
        ////////////////////////
        //      Variables     //
        ////////////////////////
        
        /*
         * I believe these are named and used logically so that they do not need XML comments.
         * Some variables keep track of similar information, which makes them redundant, 
         * but this was used to keep track of the data in a logical way 
         * and to make sure that multiple counters kept track of data in the same way. 
         * These extra variables were also used for testing. 
         * 
         * If we were to optimize the project we may be able to 
         * use variables in a more efficient way and not need as many.
         */

        // Lists, Stacks, Queues
        public Queue<Truck> entrance = new Queue<Truck>();
        public List<Dock> docks = new List<Dock>();
        public List<Truck> allTrucks = new List<Truck>();
        public List<Crate> allDeliveredCrates = new List<Crate>();
        public List<Truck> allProcessedTrucks = new List<Truck>();
        public List<Truck>[] trucks = new List<Truck>[48];

        // Warehouse
        public int timeIncrements { get; set; } = 48;
        public int currentTime { get; set; } = 0;
        public double revenue { get; set; }

        // Dock
        public int numberOfDocks { get; set; }
        public int totalUsedDockTime { get; set; }
        public int totalUnusedDockTime { get; set; }
        public int longestLine { get; set; } = 0;
        public double dockCost { get; set; } = 100;
        public double allDockSales { get; set; } = 0;
        public double avgDockTimeUse { get; set; }
        public double totalCostOfOperatingEachDock { get; set; }

        // Truck
        public int numberOfTrucks { get; set; } = 0;
        public int maxPossibleTrucksPerTimeIncrement { get; set; } = 2;
        public int totalProcessedTrucks { get; set; }
        public double totalTruckValue { get; set; }
        public double avgValueOfTrucks { get; set; }
        public float chanceOfGeneratingTruck { get; set; }

        // Crate
        public int totalCratesProcessed { get; set; }
        public double avgValueOfCrates { get; set; }


        ///////////////////////
        //      METHODS      //
        ///////////////////////
        
        /// <summary>
        /// warehouse constructor
        /// </summary>
        public Warehouse()
        {
            docks.Clear();
            entrance.Clear();
        }

            ///////////////////////
            // Run the Warehouse //
            ///////////////////////
        /// <summary>
        /// Runs the simulation
        /// </summary>
        /// <param name="warehouse">The warehouse the simulation is running</param>
        public static void Run(Warehouse warehouse)
        {
            GenerateTrucks(warehouse);

            //For loop that runs the actual simulation and updates every time increment.
            for (warehouse.currentTime = 0; warehouse.currentTime < warehouse.timeIncrements; warehouse.currentTime++) {
                AssignTrucksToDocks(warehouse);
                foreach (Dock dock in warehouse.docks)
                    ProcessDock(warehouse, dock);
            }
        }

                /////////////////////////////////////////////
                // The main methods used by the Run Method //
                /////////////////////////////////////////////
        /// <summary>
        /// Generates trucks before running the simulation
        /// </summary>
        /// <param name="warehouse">the warehouse that needs trucks</param>
        static void GenerateTrucks(Warehouse warehouse)
        {
            // creates specified number docks
            for (int i = 0; i < warehouse.numberOfDocks; i++)
            {
                Dock dock = new Dock();
                warehouse.docks.Add(dock);
            }

            // puts a list of trucks to line up each time increment
            for (int i = 0; i < warehouse.timeIncrements; i++)
                warehouse.trucks[i] = new List<Truck>();

            for (int i = 0; i < warehouse.timeIncrements; i++)
            {
                // Normal distribution code
                if (i <= warehouse.timeIncrements / 2)
                    warehouse.chanceOfGeneratingTruck = i / warehouse.timeIncrements;
                else
                    warehouse.chanceOfGeneratingTruck = (warehouse.timeIncrements - i) / (warehouse.timeIncrements / 2);
                //Attempt to make a truck a number of times equal to the max number of trucks possible per time increment.
                for (int j = 0; j < warehouse.maxPossibleTrucksPerTimeIncrement; j++)
                    if (new Random().NextDouble() >= warehouse.chanceOfGeneratingTruck)
                    {
                        //Generate a truck.
                        Truck truck = Truck.GenerateTruck();
                        warehouse.trucks[i].Add(truck);
                        warehouse.numberOfTrucks++;
                    }
            }
        }

        /// <summary>
        /// Assigns each truck to the dock with the shortest line (wait time)
        /// </summary>
        /// <param name="warehouse">the warehouse that trucks are assigned to docks at</param>
        static void AssignTrucksToDocks(Warehouse warehouse)
        {
            //Truck Arrivals at the Entrance
            foreach (Truck truck in warehouse.trucks[warehouse.currentTime])
                warehouse.entrance.Enqueue(truck);
            //Trucks Assigned to Docks
            foreach (Truck truck in warehouse.entrance) {
                int indexOfDockWithSmallestLine = FindShortestLine(warehouse);
                //Add the truck to the Dock
                warehouse.docks[indexOfDockWithSmallestLine].JoinLine(truck);
                /*
                    Note: Trucks can be added to a dock every time increment, 
                    but it doesn't say whether multiple trucks can be added to the same dock or not. 
                    This is assuming that they can in cases of small numbers of docks and a lot of trucks.
                */
            }
            warehouse.entrance.Clear();
        }

        /// <summary>
        /// Allows the Run program to process each individual dock each time increment
        /// </summary>
        /// <param name="warehouse">the warehouse that needs its docks processed</param>
        /// <param name="dock">the dock to be processed</param>
        static void ProcessDock(Warehouse warehouse, Dock dock)
        {
            /*
                processes each dock each time increment 
                by processing the truck and updating the 
                time statistics of the dock
            */
            if (dock.Line.Count > 0)
            {
                ProcessTruck(warehouse, dock);
                dock.TimeInUse++;
            }
            else
                dock.TimeNotInUse++;
        }

                    ////////////////////////////////////////////////////
                    // Helper methods used by the main methods of Run //
                    ////////////////////////////////////////////////////
        /// <summary>
        /// finds the dock with the shortest line 
        /// so that the next truck at the warehouse entrance can be sent to that line
        /// </summary>
        /// <param name="warehouse">the warehouse</param>
        /// <returns>the index of the dock with the shortest line</returns>
        static int FindShortestLine(Warehouse warehouse)
        {
            //Loop through each dock to find the one with the smallest line.
            int indexOfDockWithSmallestLine = 0;
            for (int j = 1; j < warehouse.docks.Count(); j++)
                if (warehouse.docks[j].Line.Count < warehouse.docks[indexOfDockWithSmallestLine].Line.Count())
                    indexOfDockWithSmallestLine = j;
            return indexOfDockWithSmallestLine;
        }

        /// <summary>
        /// Processes each truck at each dock at each time increment
        /// </summary>
        /// <param name="warehouse">the warehouse that trucks are being processed at</param>
        /// <param name="dock">the dock that is currently processing the truck</param>
        static void ProcessTruck(Warehouse warehouse, Dock dock)
        {
            Truck currentTruck = dock.Line.Peek();
            Crate currentCrate;

            // keeps a list of all the trucks that delivered to the factory
            if (!warehouse.allTrucks.Contains(currentTruck))
                warehouse.allTrucks.Add(currentTruck);

            /*
             * if the truck has more crates to unload,
             * it unloads it and updates variables
             */
            if (currentTruck.HasMoreCrates()) {
                currentCrate = currentTruck.Unload();
                currentCrate.timeIncrementDelivered = warehouse.currentTime;
                dock.TotalCrates++;
                dock.TotalSales += currentCrate.Price;
                currentTruck.truckWorth += currentCrate.Price;
                warehouse.allDeliveredCrates.Add(currentCrate);
                CheckNextTrucksStatus(warehouse, dock, currentTruck, currentCrate);
            }
        }

        /// <summary>
        /// A helper method to ProcessTruck()
        ///     - checks the status of the truck after a crate has been delivered
        ///     There are 3 statuses a truck could have after a crate is delivered:
        ///           HasMoreCrates - A crate was unloaded and there are more crates 
        ///                                to be delivered in the truck
        ///                                
        ///           WaitingForNextTruck - The last crate out of the back of the truck 
        ///                       has been delivered and anoter truck is in line behind this one.
        ///                       
        ///           NoNextTruck - The last crate has been delivered and there are no more truck in line
        /// </summary>
        /// <param name="warehouse">the wareohuse the truck is being processed at</param>
        /// <param name="dock">the dock the truck is being processed at</param>
        /// <param name="currentTruck">the current truck  to be processed</param>
        /// <param name="currentCrate">the current status is what happens atyer the current crate is delivered</param>
        static void CheckNextTrucksStatus(Warehouse warehouse, Dock dock, Truck currentTruck, Crate currentCrate)
        {
            /*
             * There are 3 statuses a truck could have after a crate is delivered:
             *      HasMoreCrates - A crate was unloaded and there are more crates 
             *                      to be delivered in the truck
             *          
             *      WaitingForNextTruck - The last crate out of the back of the truck
             *                            has been delivered and anoter truck is 
             *                            in line behind this one.
             *          
             *      NoNextTruck - The last crate has been delivered and 
             *                    there are no more truck in line
             */
            if (currentTruck.HasMoreCrates())
                currentCrate.scenario = "HasMoreCrates";
            else if (!currentTruck.HasMoreCrates() && dock.Line.Count() > 1) {
                warehouse.allProcessedTrucks.Add(currentTruck);
                currentCrate.scenario = "WaitingForNextTruck";
                dock.SendOff();
            }
            else {
                currentCrate.scenario = "NoNextTruck";
                dock.SendOff();
            }
        }
    }
}