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
    public class Crate
    {
        public string scenario { get; set; } = null;

        public int timeIncrementDelivered {get; set;}
        /// <summary>
        /// A integer that counts crates
        /// </summary>
        public static int crateCounter { get; set; } = 0;

        /// <summary>
        /// A string of ID's gotten by crateCounter
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// The Price of the crate between $50 and $500
        /// </summary>
        public double Price { get; private set; }

        public Crate()
        {
            Id = $"Crate_{crateCounter++}";
            Price = new Random().Next(50, 501); // Random value from $50 to $500
        }
    }
}
