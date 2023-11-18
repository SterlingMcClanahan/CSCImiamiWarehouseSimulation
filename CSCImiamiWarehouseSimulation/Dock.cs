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
      
        public int lineLength { get { return Line.Count; } }

        public int numberOfTrucksEmptied { get; set; } = 0;

        /// <summary>
        /// A integer that counts docks
        /// </summary>
        private static int dockCounter { get; set; } = 1;

        /// <summary>
        /// A string of ID's gotten by dockCounter
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A queue of trucks 
        /// </summary>
        public Queue<Truck> Line { get; set; } = new Queue<Truck>();

        /// <summary>
        /// The total amount of sales as a double
        /// </summary>
        public double TotalSales { get;  set; }

        /// <summary>
        /// The integer total of crates
        /// </summary>
        public int TotalCrates { get; set; }

        /// <summary>
        /// The integer total of trucks
        /// </summary>
        public int TotalTrucks { get; set; }

        /// <summary>
        /// How long a dock has been in use as a int
        /// </summary>
        public int TimeInUse { get; set; }

        /// <summary>
        /// How long a dock has not been used as a int
        /// </summary>
        public int TimeNotInUse { get; set; }

        ///////////////////////
        //      METHODS      //
        ///////////////////////
        
        /// <summary>
        /// The dock ID representation
        /// </summary>
        public Dock()
        {
            Id = $"{dockCounter++}";
        }

        /// <summary>
        /// Adds a truck to the line
        /// </summary>
        /// <param name="truck">The truck being added</param>
        public void JoinLine(Truck truck)
        {       
            Line.Enqueue(truck);
        }

        /// <summary>
        /// Removes a truck from the line
        /// </summary>
        /// <returns></returns>
        public Truck SendOff()
        {
            numberOfTrucksEmptied++;
            return Line.Dequeue();
        }
    }
}
