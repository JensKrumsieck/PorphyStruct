﻿using ChemSharp.Molecules.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Core;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var simSum = simulations.Sum(s => Math.Abs(s.Value));
            var expSum = part.Properties.Simulation.SimulationResult.Sum(s => Math.Abs(s.Value));

            var simSumWav = SumWav(simulations);
            var expSumWav = SumWav(part.Properties.Simulation.SimulationResult);

            foreach (var property in part.Properties.Simulation.SimulationResult.Where(s => !s.Key.Contains("Waving")))
            {
                var percentageExp = 100d * Math.Abs(property.Value) / expSum;
                var percentageSim = 100d * Math.Abs(simulations.First(s => s.Key == property.Key).Value) / simSum;
                //threshold 3%
                Assert.AreEqual(Math.Abs(percentageSim - percentageExp), 0, 3);
            }

            //compare wav by abs sum
            var wavPercentageExp = 100d * Math.Abs(expSumWav) / expSum;
            var wavPercentageSim = 100d * Math.Abs(simSumWav) / simSum;
            Assert.AreEqual(Math.Abs(wavPercentageSim - wavPercentageExp), 0, 3);

        }

        private static double SumWav(IEnumerable<KeyValueProperty> props) => props.Where(s => s.Key.Contains("Waving")).Sum(s => Math.Abs(s.Value));
    }
}