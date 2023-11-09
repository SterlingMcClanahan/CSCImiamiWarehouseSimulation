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
        /// <summary>
        /// A integer that counts docks
        /// </summary>
        private static int dockCounter = 0;

        /// <summary>
        /// A string of ID's gotten by dockCounter
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// A queue of trucks 
        /// </summary>
        public Queue<Truck> Line { get; private set; } = new Queue<Truck>();

        /// <summary>
        /// The total amount of sales as a double
        /// </summary>
        public double TotalSales { get; private set; }

        /// <summary>
        /// The integer total of crates
        /// </summary>
        public int TotalCrates { get; private set; }

        /// <summary>
        /// The integer total of trucks
        /// </summary>
        public int TotalTrucks { get; private set; }

        /// <summary>
        /// How long a dock has been in use as a int
        /// </summary>
        public int TimeInUse { get; private set; }

        /// <summary>
        /// How long a dock has not been used as a int
        /// </summary>
        public int TimeNotInUse { get; private set; }

        public Dock()
        {
            Id = $"Crate_{dockCounter++}";
        }

        public void JoinLine(Truck truck)
        {
            Line.Enqueue(truck);
        }

        public Truck SendOff()
        {
            TotalTrucks++;
            return Line.Dequeue();
        }
    }
}
