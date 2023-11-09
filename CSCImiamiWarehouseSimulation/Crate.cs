using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCImiamiWarehouseSimulation
{
    public class Crate
    {
        private static int crateCounter = 0;

        public string Id { get; private set; }
        public double Price { get; private set; }

        public Crate()
        {
            Id = $"Crate_{crateCounter++}";
            Price = new Random().Next(50, 500); // Random value from $50 to $500
        }
    }
}
