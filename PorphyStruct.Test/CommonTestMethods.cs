using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;
using System.Threading.Tasks;
using ChemSharp.Molecules.Properties;
using PorphyStruct.Core.Analysis.Properties;

namespace PorphyStruct.Test
{
    public static class CommonTestMethods
    {

        /// <summary>
        /// Runs the detection algorithm und tests for the number of detected parts
        /// </summary>
        /// <param name="path"></param>
        /// <param name="macrocycleType"></param>
        /// <param name="expectedParts"></param>
        /// <returns></returns>
        public static async Task RunDetection(string path, MacrocycleType macrocycleType, int expectedParts)
        {
            var cycle = new Macrocycle(path) { MacrocycleType = macrocycleType };
            await cycle.Detect();
            Assert.AreEqual(expectedParts, cycle.DetectedParts.Count);
        }

        /// <summary>
        /// Checks if analysis reports results similar to simulations
        /// (only 1st part comparison)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="macrocycleType"></param>
        /// <param name="simulations"></param>
        /// <returns></returns>
        public static async Task RunAnalysis(string path, MacrocycleType macrocycleType, IEnumerable<KeyValueProperty> simulations)
        {
            var viewModel = new MacrocycleViewModel(path) { Macrocycle = { MacrocycleType = macrocycleType } };
            await viewModel.Analyze();
            var part = viewModel.Macrocycle.DetectedParts[0];
            Assert.IsNotNull(part.Properties);
            foreach (var property in part.Properties.Simulation.SimulationResult)
            {
                Assert.AreEqual(property.Value, simulations.First(s => s.Key == property.Key).Value, 0.01);
            }
        }
    }
}
