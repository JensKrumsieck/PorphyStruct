using ChemSharp.Molecules.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Core.Analysis;

namespace PorphyStruct.Test
{
    [TestClass]
    public class IsoporphyrinTest
    {
        /**
         * From
        ** https://onlinelibrary.wiley.com/doi/full/10.1002/anie.201604297
        **/
        private const string TestfilesOriluyCif = "testfiles/oriluy.cif";

        private MacrocycleAnalysis _analysis;

        private static readonly List<KeyValueProperty> ExpectedResults = new()
        {
            new KeyValueProperty { Key = "Doming", Value = 0.09 },
            new KeyValueProperty { Key = "Saddling", Value = 0.99 },
            new KeyValueProperty { Key = "Ruffling", Value = 0.30 },
            new KeyValueProperty { Key = "WavingX", Value = 0 },
            new KeyValueProperty { Key = "WavingY", Value = 0.29 }, //sum wav
            new KeyValueProperty { Key = "Propellering", Value = .1 }
        };

        [TestInitialize]
        [TestMethod]
        public async Task RunDetection() => _analysis = await CommonTestMethods.RunDetection(TestfilesOriluyCif, MacrocycleType.Porphyrin, 1);

        [TestMethod]
        public void IsIsoporphyrin()
        {
            Assert.IsNotNull(_analysis);
            var porAnalysis = (PorphyrinAnalysis)_analysis;
            //Check if this isoporphyrin indeed is an isoporphyrin
            Assert.IsTrue(porAnalysis.Isoporphyrin);
        }

        /// <summary>
        /// Use 5% threshold and run sim
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestSimulation() =>
            await CommonTestMethods.RunAnalysis(TestfilesOriluyCif, MacrocycleType.Porphyrin, ExpectedResults, 5d);
    }
}
