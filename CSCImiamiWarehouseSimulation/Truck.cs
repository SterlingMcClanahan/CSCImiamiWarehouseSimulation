using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCImiamiWarehouseSimulation
{
    internal class Truck
    {
        /* 
         * string driver - The driver’s name.
            string deliveryCompany – The delivery company that the driver is working for.
            Stack<Crate> Trailer - The crates in the truck’s trailer.
            The Truck class should implement the following methods:
            void Load(Crate crate) - Adds a crate to the truck’s trailer, being sure to load from back
            to front.
            Crate Unload() – Removes a crate from the front of the truck’s trailer, moving from front
            to back
        */

        public string driver;
        public string deliveryCompany;
        public Stack<Crate> Trailer;

        public Truck()
        {
            driver = string.Empty;
            deliveryCompany = string.Empty;
            Stack<Crate> Trailer = new Stack<Crate>();
        }

        public Truck(string driver, string deliveryCompany)
        {
            this.driver = driver;
            this.deliveryCompany = deliveryCompany;
            Stack<Crate> Trailer = new Stack<Crate>();
        }

        //might have to do a special case for first item (not sure)
        public void Load(Crate crate)
        {
            Trailer.Push(crate);
        }

        //might have to do a special case to check if there is any item to remove (not sure)
        public Crate Unload(Crate crate)
        {
            return Trailer.Pop();
        }
    }
}
