///////////////////////////////////////////////////////////////////////////////
//
// Author: Daniel Foister, Curtis Reece, Sterling McClanahan, Chris Oaks
// Course: CSCI-2210-001 - Data Structures
// Assignment: Project3 - Warehouse Simulation
// Description: A demonstration of our understanding and proficiency in the data structures we have discussed in class,
//              in the form of a warehouse simulation.
//
///////////////////////////////////////////////////////////////////////////////
using System.Reflection.Metadata.Ecma335;

namespace CSCImiamiWarehouseSimulation
{
    //Needs to be tested to ensure it actually works. I also still need to go to Gillenwater's office hours to see if this is the correct way to do this.
    //Where I discovered how to do this --> https://forum.unity.com/threads/random-number-with-normal-distribution-passing-average-value.1229193/
    public class NormalDistribution
    {
        private double prevRandomValue;
        public double Mean { get; set; }
        public double Variance { get; set; }
        public double StandardDev { get; set; }

        public NormalDistribution(double mean, double standardDev)
        {
            this.Mean = mean;
            this.StandardDev = standardDev;
            Variance = standardDev * standardDev;
        }

        public double Sample(Random rand)
        {
            double x1, x2, w, y1, y2;

    
                do
                {
                    x1 = 2.0 * rand.NextDouble() - 1.0;
                    x2 = 2.0 * rand.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                }
                while (w >= 1.0);

                w = Math.Sqrt(-2.0 * Math.Log(w) / w);
                y1 = x1 * w;
            

            return Mean + y1 * StandardDev;
        }

        /// <summary>
        /// <seealso cref="https://www.scirp.org/journal/paperinformation.aspx?paperid=96275"/>
        /// 
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public double SampleRev2(Random rand)
        {
            var zeroToOne = rand.NextDouble();
            var result = Math.Log(1 - zeroToOne);
            var lambda = -1;
            return lambda * result;
        }

    }
}
