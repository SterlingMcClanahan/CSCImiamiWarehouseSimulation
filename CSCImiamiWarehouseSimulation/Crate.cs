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

namespace CSCImiamiWarehouseSimulation
{
    public class Crate
    {
        ////////////////////////
        //      Variables     //
        ////////////////////////
        
        /// <summary>
        /// Keeps track of the time increment that the crate was delivered
        /// </summary>
        public int timeIncrementDelivered {get; set;}

        /// <summary>
        /// keeps a count of how many crates have been created and is used as each crates id
        /// </summary>
        public static int crateCounter { get; set; } = 0;

        /// <summary>
        /// The value of each crate, this is the warehouses profit
        /// </summary>
        public double price { get; private set; }

        /// <summary>
        /// Each crates id in the form of a string to be printed to a csv later
        /// </summary>
        public string id { get; private set; }

        /// <summary>
        /// There are 3 statuses a truck could have after a crate is delivered:
        ///     HasMoreCrates - A crate was unloaded and there are more crates
        ///                     to be delivered in the truck
        ///     WaitingForNextTruck - The last crate out of the back of the truck 
        ///                           has been delivered and anoter truck is 
        ///                           in line behind this one.
        ///     NoNextTruck - The last crate has been delivered and
        ///                   there are no more truck in line
        /// This status is assigned to the previous crate delivered.
        /// </summary>
        public string scenario { get; set; } = null;


        ///////////////////////
        //      METHODS      //
        ///////////////////////

        /// <summary>
        /// Constructor for each Crate
        /// </summary>
        public Crate()
        {
            id = $"Crate_{crateCounter++}";
            price = new Random().Next(50, 501); // Random value from $50 to $500
        }
    }
}
