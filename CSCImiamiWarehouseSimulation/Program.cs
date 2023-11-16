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
            //make it simple to check output
            warehouse.numberOfDocks = 6;
            warehouse.numberOfTrucks = 1;
            //warehouse.maxPossibleTrucksPerTimeIncrement = 1;
        

            Warehouse.Run(warehouse);
            Warehouse.PrintEverything(warehouse);
        }
    }
}