using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Chemistry.Macrocycles;
using System.Linq;

namespace PorphyStruct.Test
{
    [TestClass]
    public class CorroleTest
    {
        private Corrole corrole { get; set; }

        /// <summary>
        /// Loads Corrole
        /// </summary>
        [TestInitialize]
        public void Initialize() => corrole = MacrocycleSetup.CreateTestCorroleCIF();

        /// <summary>
        /// Checks validity 
        /// </summary>
        [TestMethod]
        public void TestValidity() => Assert.IsTrue(corrole.IsValid);
        
        /// <summary>
        /// Checks if Chromium is detected
        /// </summary>
        [TestMethod]
        public void TestHasMetal() => Assert.IsTrue(corrole.HasMetal(false));

        /// <summary>
        /// Tests datapoint related stuff
        /// </summary>
        [TestMethod]
        public void TestDataPoints() { Assert.IsNull(corrole.DataPoints);
            corrole.GetDataPoints();
            Assert.IsNotNull(corrole.DataPoints);
            Assert.AreEqual(corrole.RingAtoms.Count, corrole.DataPoints.Count);
            //as there should be only one provider!
            Assert.AreEqual(corrole.Bonds.Count, corrole.DrawBonds(corrole.DataProviders.First()).Count());
        }

        /// <summary>
        /// Checks whether both methods give equal results
        /// </summary>
        [TestMethod]
        public void TestCifvsMol2()
        {
            var CIF = MacrocycleSetup.CreateTestCorroleCIF();
            var Mol2 = MacrocycleSetup.CreateTestCorroleMol2();
            //Test Length of Atomcount
            Assert.AreEqual(CIF.Atoms.Count, Mol2.Atoms.Count, 0, "The Methods extract different numbers of atoms");

            //Test validity (Files are prevalidated!)
            Assert.IsTrue(CIF.IsValid);
            Assert.IsTrue(Mol2.IsValid);

            //Test Datapoint equality
            CIF.GetDataPoints();
            Mol2.GetDataPoints();


            //using thresholds here because mercury's conversion may does other rounding
            for (int i = 0; i < CIF.DataPoints.Count; i++)
            {
                Assert.AreEqual(CIF.DataPoints[i].Y, Mol2.DataPoints[i].Y, 0.0005, $"Non Equality at Y:{i}");
                Assert.AreEqual(CIF.DataPoints[i].X, Mol2.DataPoints[i].X, 0.0005, $"Non Equality at X:{i}");
                Assert.AreEqual(CIF.DataPoints[i].atom.Identifier, Mol2.DataPoints[i].atom.Identifier, $"Non Equality at atom:{i}");
            }

            Assert.AreEqual((double)CIF.MeanDisplacement(), (double)Mol2.MeanDisplacement(), 0.0001, "Mean Displacement not equal");
        }
    }
}
