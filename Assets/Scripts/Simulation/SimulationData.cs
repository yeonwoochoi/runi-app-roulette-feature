using System;
using System.Collections.Generic;
using Generator;

namespace Simulation
{
    public class SimulationData
    {
        public int index;
        public RandomItemType expectItemType;
        public RandomItemType acceptItemType;
        public bool IsMatch => expectItemType == acceptItemType;
        public string Time => DateTime.Now.ToString();
        public float lerpValue;

        public SimulationData(int id, RandomItemType expect, RandomItemType accept, float lerpValue)
        {
            index = id;
            expectItemType = expect;
            acceptItemType = accept;
            this.lerpValue = lerpValue;
        }

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