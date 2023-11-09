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
        private static int crateCounter = 0;

        public string Id { get; private set; }

        public Queue<Truck> Line { get; private set; } = new Queue<Truck>();
        public double TotalSales { get; private set; }
        public int TotalCrates { get; private set; }
        public int TotalTrucks { get; private set; }
        public int TimeInUse { get; private set; }
        public int TimeNotInUse { get; private set; }

        public Dock()
        {
            Id = $"Crate_{crateCounter++}";
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
