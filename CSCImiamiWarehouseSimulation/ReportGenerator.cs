using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCImiamiWarehouseSimulation
{
    internal class ReportGenerator
    {
        ///////////////////////
        //      METHODS      //
        ///////////////////////
        
        ///
        /// <summary>
        /// Clears the previous data from the CSV so that new data can take its place.
        /// </summary>
        public static void ClearCSV()
        {
            using (StreamWriter writer = new StreamWriter(@"crateData.csv", false))
            {

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
        public static void LogToCSV(int timeIncrement, string driver, string company, string id, double price, string scenario)
        {
            //Does not clear the previous CSV file before making a new one

            // Replace "yourfile.csv" with the actual path and filename you want to use
            string filePath = "crateData.csv";

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
        /// updates all variables and calculates new ones.
        /// </summary>
        /// <param name="warehouse">the warehouse to pull data from</param>
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
            Console.WriteLine("Report: \n" +
                $"Number of Docks: {warehouse.numberOfDocks} \n" +
                $"Longest Line: {warehouse.longestLine} \n" +
                $"Toal Trucks Processed: {warehouse.totalProcessedTrucks} \n" +
                $"Total Crates Processed: {warehouse.allDeliveredCrates.Count()}\n" +
                $"Total Sales From all Docks: {warehouse.allDockSales} \n" +
                $"Average Value of Each Crate: {warehouse.avgValueOfCrates} \n" +
                $"Average Value of Each Truck: {warehouse.avgValueOfTrucks} \n" +
                $"Total Time Used by Docks: {warehouse.totalUsedDockTime} \n" +
                $"Total Time Unused by Docks: {warehouse.totalUnusedDockTime} \n" +
                $"Average Time Each Dock Was in Use: {warehouse.avgDockTimeUse} \n" +
                $"Total Cost of Operating Each Dock: {warehouse.totalCostOfOperatingEachDock} \n" +
                $"Total Revenue: {warehouse.revenue} \n"
                );
            Console.WriteLine("All Dock Reports: ");
            foreach (Dock dock in warehouse.docks)
                Console.WriteLine
                    ($"  Dock: {dock.Id}\n" +
                    $"    Total Trucks Processed: {dock.numberOfTrucksEmptied} \n" +
                    $"    Total Crates Processed: {dock.TotalCrates} \n" +
                    $"    Total Time Used: {dock.TimeInUse} \n" +
                    $"    Total Time Not Used: {dock.TimeNotInUse} \n" +
                    $"    Total Sales: {dock.TotalSales}"
                    );
            //this goes to the csv file
            foreach (Truck truck in warehouse.allTrucks)
            {
                warehouse.totalTruckValue += truck.truckWorth;
                foreach (Crate crate in truck.deliveredCrates)
                    LogToCSV(crate.timeIncrementDelivered, truck.driver, truck.deliveryCompany, crate.Id, crate.Price, crate.scenario);
            }
        }
    }
}
