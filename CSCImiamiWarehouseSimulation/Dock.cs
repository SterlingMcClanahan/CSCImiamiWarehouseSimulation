using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCImiamiWarehouseSimulation
{
    internal class Dock
    {
        public string Id { get; private set; }
        public Queue<Truck> Line { get; private set; } = new Queue<Truck>();
        public double TotalSales { get; private set; }
        public int TotalCrates { get; private set; }
        public int TotalTrucks { get; private set; }
        public int TimeInUse { get; private set; }
        public int TimeNotInUse { get; private set; }

        public Dock()
        {
            Id = Guid.NewGuid().ToString();
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
