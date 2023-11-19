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
    internal class Dock
    {
        ////////////////////////
        //      Variables     //
        ////////////////////////

        /// <summary>
        /// A queue of trucks 
        /// </summary>
        public Queue<Truck> line { get; set; } = new Queue<Truck>();

        /// <summary>
        /// The current length of line at the dock, 
        /// so the warehouse knows where to assign the next truck
        /// </summary>
        public int lineLength { get { return line.Count; } }

        /// <summary>
        /// Keeps track of how many trucks the dock has emptied
        /// </summary>
        public int numberOfTrucksEmptied { get; set; } = 0;

        /// <summary>
        /// A integer that counts docks
        /// </summary>
        private static int dockCounter { get; set; } = 1;

        /// <summary>
        /// The integer total of crates
        /// </summary>
        public int totalCrates { get; set; }

        /// <summary>
        /// The integer total of trucks
        /// </summary>
        public int totalTrucks { get; set; }

        /// <summary>
        /// How long a dock has been in use as a int
        /// </summary>
        public int timeInUse { get; set; }

        /// <summary>
        /// How long a dock has not been used as a int
        /// </summary>
        public int timeNotInUse { get; set; }

        /// <summary>
        /// The total amount of sales as a double
        /// </summary>
        public double totalSales { get; set; }

        /// <summary>
        /// A string of ID's gotten by dockCounter
        /// </summary>
        public string id { get; set; }

        ///////////////////////
        //      METHODS      //
        ///////////////////////

        /// <summary>
        /// The dock constructor
        /// </summary>
        public Dock()
        {
            id = $"{dockCounter++}";
        }

        /// <summary>
        /// Adds a truck to the line
        /// </summary>
        /// <param name="truck">The truck being added</param>
        public void JoinLine(Truck truck)
        {       
            line.Enqueue(truck);
        }

        /// <summary>
        /// Removes a truck from the line
        /// </summary>
        /// <returns></returns>
        public Truck SendOff()
        {
            numberOfTrucksEmptied++;
            return line.Dequeue();
        }
    }
}
