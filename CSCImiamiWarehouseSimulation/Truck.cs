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
    internal class Truck
    {
        public List<Crate> deliveredCrates = new List<Crate>();
        public int truckCounter = 0;
        public int id;

        public double truckWorth = 0;
        /// <summary>
        /// The name of the truck driver associated with the truck.
        /// </summary>
        public string driver { get; private set; }

        /// <summary>
        /// The name of the delivery company associated with the truck.
        /// </summary>
        public string deliveryCompany { get; private set; }

        /// <summary>
        /// The stack of crates inside of the truck.
        /// </summary>
        public Stack<Crate> Trailer = new Stack<Crate>();
       
        /// <summary>
        /// A list of potential driver names from the 1994 Japanese Baseball game "Fighting Baseball".
        /// </summary>
        public static List<string> driverNames = new List<string>
            {
                "Sleve McDichael", "Onson Sweemey", "Darryl Archideld",
                "Anatoli Smorin", "Rey McSriff" ,"Glenallen Mixon",
                "Mario McRlwain", "Raul Chamgerlain", "Kevin Nogilny",
                "Tony Smehrik", "Bobson Dugnutt", "Willie Dustice",
                "Jeromy Gride", "Scott Dourque", "Shown Furcotte",
                "Dean Wesrey", "Mike Truk", "Dwigt Rortugal",
                "Tim Sandaele", "Karl Dandleton", "Mike Sernandez",
                "Todd Bonzalez"
            };

        /// <summary>
        /// A list of potential delivery company names. (Copied from Github).
        /// </summary>
        public static List<string> deliveryCompanies = new List<string>
            {
                "Acme Corporation", "Globex Corporation", "Soylent Corp",
                "Initech", "Bluth Company", "Umbrealla Corporation",
                "Hooli", "Duff Beer", "Massive Dynamic",
                "Wonka Industries", "Stark Industries", "Gekko & Co",
                "Wayne Enterprises", "Bubba Gump", "Cyberdyne Systems",
                "Genco Pura Olive Oil Company", "The New York Inquirer"
            };

        /// <summary>
        /// Minimum number of crates for simulation
        /// </summary>
        public static int minimumCrateNumber = 3;
        /// <summary>
        /// Maximum number of crates for simulation
        /// </summary>
        public static int maximumCrateNumber = 8;

        /// <summary>
        /// Default constructor for the truck.
        /// </summary>
        public Truck()
        {
            driver = string.Empty;
            deliveryCompany = string.Empty;
            id = truckCounter++;
            Stack<Crate> Trailer = new Stack<Crate>();
        }

        /// <summary>
        /// Parameterized constructor for the truck.
        /// </summary>
        /// <param name="driver">The name of the truck driver associated with the truck.</param>
        /// <param name="deliveryCompany">The name of the delivery company associated with the truck.</param>
        public Truck(string driver, string deliveryCompany)
        {
            this.driver = driver;
            this.deliveryCompany = deliveryCompany;
            Stack<Crate> Trailer = new Stack<Crate>();
        }

        /// <summary>
        /// Generates a truck using the parameterized constructor and a random driver name and delivery company from a preset list of names.
        /// </summary>
        /// <returns></returns>
        public static Truck GenerateTruck()
        {
            Random random = new Random();

            //Generate random driver and company names and use the parameterized constructor.
            Truck truck = new Truck(driverNames[random.Next(0, Truck.driverNames.Count - 1)], deliveryCompanies[random.Next(0, Truck.deliveryCompanies.Count - 1)]);

            //Add a random amount of crates to the truck.
            for (int i = 0; i < new Random().Next(minimumCrateNumber, maximumCrateNumber); i++)
            {
                Crate crate = new Crate();
                truck.Load(crate);
            }

            return truck;
        }

        /// <summary>
        /// A method that loads crates into the trailer by pushing them onto a stack.
        /// </summary>
        /// <param name="crate">The crate that was pushed onto the stack.</param>
        public void Load(Crate crate)
        {
            Trailer.Push(crate);
        }

        /// <summary>
        /// A method that removes crates from the trailer by popping them from the stack.
        /// </summary>
        /// <returns>The crate the was popped from the stack.</returns>
        /// <exception cref="IndexOutOfRangeException">An exception thrown when trying to remove crates from an empty stack.</exception>
        public Crate Unload()
        {
            if (HasMoreCrates())
            {
                deliveredCrates.Add(Trailer.Peek());
                return Trailer.Pop();
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
            
        }

        /// <summary>
        /// A method that determines if the truck has more crates in its trailer.
        /// </summary>
        /// <returns>A boolean value of true if there are more crates and false if there are not any more crates.</returns>
        public bool HasMoreCrates()
        {
            if (Trailer.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
