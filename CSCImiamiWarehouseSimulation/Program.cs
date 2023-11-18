///////////////////////////////////////////////////////////////////////////////
//
// Author: Daniel Foister, Curtis Reece, Sterling McClanahan, Chris Oaks
// Course: CSCI-2210-001 - Data Structures
// Assignment: Project3 - Warehouse Simulation
// Description: A demonstration of our understanding and proficiency in the data structures we have discussed in class,
//              in the form of a warehouse simulation.
//
///////////////////////////////////////////////////////////////////////////////
namespace CSCImiamiWarehouseSimulation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Warehouse warehouse = new Warehouse();
            warehouse.numberOfDocks = 5;
            Warehouse.Run(warehouse);

            // This will clear the csv. If you want to append, then comment this out.
            ReportGenerator.ClearCSV();
            // Calculates the data but does nothing with it yet
            ReportGenerator.CalculateData(warehouse);
            // Prints a report to command line and creates a csv file of the crates delivered
            ReportGenerator.GenerateReport(warehouse);
        }
    }
}