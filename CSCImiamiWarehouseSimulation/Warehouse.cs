///////////////////////////////////////////////////////////////////////////////
//
// Author: Daniel Foister, Curtis Reece, Sterling McClanahan, Chris Oaks
// Course: CSCI-2210-001 - Data Structures
// Assignment: Project3 - Warehouse Simulation
// Description: A demonstration of our understanding and proficiency in the data structures we have discussed in class,
//              in the form of a warehouse simulation.
//
///////////////////////////////////////////////////////////////////////////////
using System.Security.Cryptography.X509Certificates;

namespace CSCImiamiWarehouseSimulation
{
    internal class Warehouse
    {

        List<Dock> docks = new List<Dock>();
        Queue<Truck> entrance = new Queue<Truck>();

        public Warehouse()
        {
            docks.Clear();
            entrance.Clear();
        }

        public static void Run()
        {
            //Initial Setup of Simulation and its parameters.
            int timeIncrements = 48; //Each increment represents 10 minutes out of an 8 hour shift.
            int currentTime = 0;
            int numberOfDocks = 10;
            int numberOfTrucks = 0;
            float chanceOfGeneratingTruck = 0;
            int maxPossibleTrucksPerTimeIncrement = 5;

            //Setup of Warehouse
            Warehouse warehouse = new Warehouse();


            //Setup of Docks
            for (int i = 0; i < numberOfDocks; i++)
            {
                Dock dock = new Dock();
                warehouse.docks.Add(dock);
            }

            //Setup of Trucks
            List<Truck>[] trucks = new List<Truck>[timeIncrements];

            for (int i = 0; i < timeIncrements; i++)
            {
                trucks[i] = new List<Truck>();
            }

            //For the number of Time Increments.
            for(int i = 0; i < timeIncrements; i++)
            {
                //Determine how many trucks per time increment.

                //This is where we need to do the normal distribution code.
                if(i < timeIncrements / 2)
                {
                    chanceOfGeneratingTruck = i / timeIncrements;
                }
                else
                {
                    chanceOfGeneratingTruck = (timeIncrements - i) / (timeIncrements / 2);
                }

                //Attempt to make a truck a number of times equal to the max number of trucks possible per time increment.
                for(int j = 0; j < maxPossibleTrucksPerTimeIncrement; j++)
                {
                    if (new Random().NextDouble() >= chanceOfGeneratingTruck)
                    {
                        //Generate a truck.
                        Truck truck = Truck.GenerateTruck();
                        trucks[i].Add(truck);
                    }
                }
            }

            //For loop that runs the actual simulation and updates every time increment.
            for (currentTime = 0; currentTime < timeIncrements; currentTime++)
            {
                //Truck Arrivals at the Entrance
                foreach (Truck truck in trucks[currentTime])
                {
                    warehouse.entrance.Enqueue(truck);
                }

                //Trucks Assigned to Docks
                foreach (Truck truck in warehouse.entrance)
                {
                    int indexOfDockWithSmallestLine = 0;
                    //Loop through each dock to find the one with the smallest line.
                    for (int j = 1; j < warehouse.docks.Count; j++)
                    {
                        if (warehouse.docks[j].Line.Count < warehouse.docks[indexOfDockWithSmallestLine].Line.Count)
                        {
                            indexOfDockWithSmallestLine = j;
                        }
                    }

                    //Add the truck to the Dock
                    warehouse.docks[indexOfDockWithSmallestLine].JoinLine(truck);
                    //Note: Trucks can be added to a dock every time increment, but it doesn't say whether multiple trucks
                    //can be added to the same dock or not. This is assuming that they can in cases of small numbers of docks and a lot of trucks.
                }

                //Process the docks by unloading a crate.
                //Handle the scenario of swapping to the next queued truck if last crate was unloaded.
                foreach (Dock dock in warehouse.docks)
                {
                    Truck currentTruck;
                    if (dock.Line.Count > 0)
                    {
                        currentTruck = dock.Line.Peek();
                        if (currentTruck.HasMoreCrates())
                        {
                            Crate currentCrate = currentTruck.Unload();
                            dock.TotalCrates++;
                            dock.TotalSales += currentCrate.Price;
                        }

                        if (currentTruck.HasMoreCrates())
                        {
                            //Situation where crate has been unloaded and there are more crates to unload.
                            //Do nothing currently, but eventually add logging info here and nothing else.
                        }
                        else
                        {
                            //Situation where crate has been unloaded and the truck has no more crates to unload.
                            dock.SendOff();
                            dock.TotalTrucks++;
                            
                            if (dock.Line.Count > 0)
                            {
                                //And another truck is already in the Dock
                                //Do nothing currently, but eventually add logging info here and nothing else.
                            }
                            else if (dock.Line.Count == 0)
                            {
                                //But another truck is NOT already in the Dock
                                //Do nothing currently, but eventually add logging info here and nothing else.
                            }
                        }

                        dock.TimeInUse++;
                    }
                    else
                    {
                        dock.TimeNotInUse++;
                    }
                }


            }

        }

    }
}
