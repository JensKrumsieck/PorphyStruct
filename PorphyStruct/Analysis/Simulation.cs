﻿using ChemSharp.Mathematics;
using ChemSharp.Molecules.DataProviders;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PorphyStruct.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Analysis
{
    public class Simulation
    {
        private readonly List<(string name, string path)> _porphyrinPaths = new List<(string name, string path)>()
        {
            ("Doming", "PorphyStruct.Reference.Porphyrin.Doming.xyz"),
            ("Saddling", "PorphyStruct.Reference.Porphyrin.Saddling.xyz"),
            ("Ruffling", "PorphyStruct.Reference.Porphyrin.Ruffling.xyz"),
            ("WavingX", "PorphyStruct.Reference.Porphyrin.WavingX.xyz"),
            ("WavingY", "PorphyStruct.Reference.Porphyrin.WavingY.xyz"),
            ("Waving2X", "PorphyStruct.Reference.Porphyrin.Waving2X.xyz"),
            ("Waving2Y", "PorphyStruct.Reference.Porphyrin.Waving2Y.xyz"),
            ("Propellering", "PorphyStruct.Reference.Porphyrin.Propellering.xyz")
        };

        private readonly List<(string name, string path)> _norcorrolePaths = new List<(string name, string path)>()
        {
            ("Doming", "PorphyStruct.Reference.Norcorrole.Doming.xyz"),
            ("Saddling", "PorphyStruct.Reference.Norcorrole.Saddling.xyz"),
            ("Ruffling", "PorphyStruct.Reference.Norcorrole.Ruffling.xyz"),
            ("WavingX", "PorphyStruct.Reference.Norcorrole.WavingDipy.xyz"),
            ("WavingY", "PorphyStruct.Reference.Norcorrole.WavingBipy.xyz"),
            ("Propellering", "PorphyStruct.Reference.Norcorrole.Propellering.xyz")
        };

        public Matrix<double> ReferenceMatrix { get; private set; }

        /// <summary>
        /// Creates a Simulation Object for given Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="minimal">for Porphyrins only - will reduce basis by Waving 2 to fit Shelnutt et. al.</param>
        public Simulation(MacrocycleType type, bool minimal = false)
        {
            switch (type)
            {
                case MacrocycleType.Porphyrin:
                    BuildPorphyrinMatrix(minimal);
                    break;
                case MacrocycleType.Norcorrole:
                    ReferenceMatrix = DisplacementMatrix(_norcorrolePaths.Select(s => s.path), type);
                    break;
            }
        }

        /// <summary>
        /// Calculates Simulation for given Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] Simulate(double[] data) => ReferenceMatrix.QR().Solve(DenseVector.OfArray(data)).ToArray();

        /// <summary>
        /// Build Displacement Matrix from Array of Paths (in Resources)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Matrix<double> DisplacementMatrix(IEnumerable<string> res, MacrocycleType type)
        {
            var mat = new List<double[]>();
            foreach (var s in res)
            {
                var stream = ResourceUtil.LoadResource(s);
                var xyz = new XYZDataProvider(stream);
                var cycle = new Macrocycle(xyz) { MacrocycleType = type };
                Task.Run(cycle.Detect).Wait(500);
                var part = cycle.DetectedParts[0];
                var data = part.DataPoints.OrderBy(d => d.X).Select(d => d.Y).ToArray();
                mat.Add(data.Normalize());
            }
            return Matrix.Build.DenseOfColumnArrays(mat);
        }

        /// <summary>
        /// Porphyrins can request minimal basis discarding waving2 modes
        /// </summary>
        /// <param name="minimal"></param>
        private void BuildPorphyrinMatrix(bool minimal = false)
        {
            ReferenceMatrix = DisplacementMatrix(_porphyrinPaths.Select(s => s.path), MacrocycleType.Porphyrin);
            if (minimal)
                ReferenceMatrix =
                    DisplacementMatrix(_porphyrinPaths.Where(s => !s.name.Contains("2")).Select(s => s.path), MacrocycleType.Porphyrin);
        }
    }
}
