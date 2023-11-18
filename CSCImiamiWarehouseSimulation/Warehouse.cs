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
        

        public List<Dock> docks = new List<Dock>();
        public Queue<Truck> entrance = new Queue<Truck>();
        public List<Truck> allTrucks = new List<Truck>();
        public List<int> allTruckIds = new List<int>();
        public List<Crate> allDeliveredCrates = new List<Crate>();
        public List<Truck> allProcessedTrucks = new List<Truck>();
        public List<Truck>[] trucks = new List<Truck>[48];
        public double dockCost { get; set; } = 100;
        public int timeIncrements { get; set; } = 48;
        public int currentTime { get; set; } = 0;
        public int numberOfDocks { get; set; }
        public int numberOfTrucks { get; set; } = 0;
        public float chanceOfGeneratingTruck {  get; set; }
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


        ///////////////////////
        //      METHODS      //
        ///////////////////////
        

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
            GenerateTrucks(warehouse);

            //For loop that runs the actual simulation and updates every time increment.
            for (warehouse.currentTime = 0; warehouse.currentTime < warehouse.timeIncrements; warehouse.currentTime++) {
                AssignTrucksToDocks(warehouse);
                foreach (Dock dock in warehouse.docks)
                    ProcessDock(warehouse, dock);
            }
        }

        static void ProcessTruck(Warehouse warehouse, Dock dock)
        {
            Truck currentTruck;
            Crate currentCrate;

            currentTruck = dock.Line.Peek();
            if (!warehouse.allTrucks.Contains(currentTruck)) {
                warehouse.allTrucks.Add(currentTruck);
                warehouse.allTruckIds.Add(currentTruck.id);
            }

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

        static void CheckNextTrucksStatus(Warehouse warehouse, Dock dock, Truck currentTruck, Crate currentCrate)
        {
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

        /// <summary>
        /// Allows the Run program to process each individual dock each time increment
        /// </summary>
        /// <param name="warehouse">the warehouse that needs its docks processed</param>
        /// <param name="dock">the dock to be processed</param>
        static void ProcessDock(Warehouse warehouse, Dock dock)
        {
            if (dock.Line.Count > 0) {
                ProcessTruck(warehouse, dock);
                dock.TimeInUse++;
            }
            else
                dock.TimeNotInUse++;
        }

        /// <summary>
        /// Generates trucks before running the simulation
        /// </summary>
        /// <param name="warehouse">the warehouse that needs trucks</param>
        static void GenerateTrucks(Warehouse warehouse)
        {
            for (int i = 0; i < warehouse.numberOfDocks; i++) {
                Dock dock = new Dock();
                warehouse.docks.Add(dock);
            }

            for (int i = 0; i < warehouse.timeIncrements; i++)
                warehouse.trucks[i] = new List<Truck>();

            for (int i = 0; i < warehouse.timeIncrements; i++) {
                //This is where we need to do the normal distribution code. <--Might need this
                if (i <= warehouse.timeIncrements / 2)
                    warehouse.chanceOfGeneratingTruck = i / warehouse.timeIncrements;
                else
                    warehouse.chanceOfGeneratingTruck = (warehouse.timeIncrements - i) / (warehouse.timeIncrements / 2);
                //Attempt to make a truck a number of times equal to the max number of trucks possible per time increment.
                for (int j = 0; j < warehouse.maxPossibleTrucksPerTimeIncrement; j++)
                    if (new Random().NextDouble() >= warehouse.chanceOfGeneratingTruck) {
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
                int indexOfDockWithSmallestLine = 0;
                //Loop through each dock to find the one with the smallest line.
                for (int j = 1; j < warehouse.docks.Count(); j++)
                    if (warehouse.docks[j].Line.Count < warehouse.docks[indexOfDockWithSmallestLine].Line.Count())
                        indexOfDockWithSmallestLine = j;
                //Add the truck to the Dock
                warehouse.docks[indexOfDockWithSmallestLine].JoinLine(truck);
                //Note: Trucks can be added to a dock every time increment, but it doesn't say whether multiple trucks
                //can be added to the same dock or not. This is assuming that they can in cases of small numbers of docks and a lot of trucks.
            }
            warehouse.entrance.Clear();
        }

    }
}