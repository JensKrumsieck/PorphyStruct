using ChemSharp.Molecules.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Core.Analysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PorphyStruct.Test
{
    [TestClass]
    public class Shelnutt_1997_Test
    {
        /// <summary>
        /// 3 selected cases from 1997 original shelnutt paper:
        /// https://pubs.acs.org/doi/full/10.1021/jp963142h
        /// </summary>
        private static string[] shelnuttFiles = new string[]
        {
            "testfiles/CuHETMP.cif",
            "testfiles/NidPTETMP.cif",
            "testfiles/Zn(py)TNPCP.cif"
        };

        private static readonly List<KeyValueProperty> CuHETMP = new List<KeyValueProperty>
        {
            new KeyValueProperty{ Key = "Doming", Value = 0.293},
            new KeyValueProperty {Key = "Saddling", Value = -0.182},
            new KeyValueProperty {Key ="Ruffling", Value = 1.692},
            new KeyValueProperty {Key = "WavingX", Value = -0.137},
            new KeyValueProperty {Key = "WavingY", Value = 0.028},
            new KeyValueProperty {Key = "Propellering", Value = -0.024}
        };

        private static readonly List<KeyValueProperty> NidPTETMP = new List<KeyValueProperty>
        {
            new KeyValueProperty{ Key = "Doming", Value = 0},
            new KeyValueProperty {Key = "Saddling", Value = -1.192},
            new KeyValueProperty {Key ="Ruffling", Value = 0},
            new KeyValueProperty {Key = "WavingX", Value = 0.154},
            new KeyValueProperty {Key = "WavingY", Value =0.154},
            new KeyValueProperty {Key = "Propellering", Value = 0}
        };

        private static readonly List<KeyValueProperty> ZnTPCP = new List<KeyValueProperty>
        {
            new KeyValueProperty{ Key = "Doming", Value = 0.669},
            new KeyValueProperty {Key = "Saddling", Value = 0.126},
            new KeyValueProperty {Key ="Ruffling", Value = 0.041},
            new KeyValueProperty {Key = "WavingX", Value = 0.086},
            new KeyValueProperty {Key = "WavingY", Value =-0.057},
            new KeyValueProperty {Key = "Propellering", Value = 0.008}
        };

        private static readonly List<KeyValueProperty>[] SimulationData =
        {
            CuHETMP,
            NidPTETMP,
            ZnTPCP
        };
        [TestMethod]
        public async Task RunTests()
        {
            foreach (var file in shelnuttFiles)
            {
                await CommonTestMethods.RunAnalysis(file, MacrocycleType.Porphyrin,
                    SimulationData[Array.IndexOf(shelnuttFiles, file)]);
            }
        }
    }

}
