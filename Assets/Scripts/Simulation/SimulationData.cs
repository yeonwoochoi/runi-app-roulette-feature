using System;
using Generator;

namespace Simulation
{
    /// <summary>
    /// Data about the simulation is as fields in this class.
    /// </summary>
    public class SimulationData
    {
        /// <value> Corresponding to the number of simulations. </value>
        public int index;
        
        /// <value> Expected reward item type. </value>
        public RandomItemType expectItemType;
        
        /// <value> Reward item type actually received. </value>
        public RandomItemType acceptItemType;
        
        /// <value> Whether <c>expectItemType</c> and <c>acceptItemType</c> match. </value>
        public bool IsMatch => expectItemType == acceptItemType;
        
        /// <value> The time the log is stamped. </value>
        public string Time => DateTime.Now.ToString();
        
        /// <value> Initial lerp value. </value>
        public float lerpValue;

        
        public SimulationData(int id, RandomItemType expect, RandomItemType accept, float lerpValue)
        {
            index = id;
            expectItemType = expect;
            acceptItemType = accept;
            this.lerpValue = lerpValue;
        }

        /// <summary>
        /// Convert the <c>SimulationData</c> instance into an array to save it to a csv file.
        /// This method converts the <c>SimulationData</c> instance that is created when the simulation runs for one cycle.
        /// </summary>
        /// <returns>This array is used as a parameter of <c>CsvController.WriteToCsv</c>.</returns>
        /// <seealso cref="CsvController.WriteToCsv"/>
        public string[] ToArray()
        {
            return new string[]
            {
                index.ToString(),
                Time,
                expectItemType.ToString(),
                acceptItemType.ToString(),
                IsMatch ? "true" : "false",
                lerpValue.ToString()
            };
        }
    }
}