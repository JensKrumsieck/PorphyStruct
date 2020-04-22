using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Chemistry;
using PorphyStruct.Chemistry.Macrocycles;
using PorphyStruct.Chemistry.Properties;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Test
{
    [TestClass]
    public class MathTest
    {
        private Corrole corrole { get; set; } = MacrocycleSetup.CreateTestCorroleCIF();

        /// <summary>
        /// Double checks Crossproduct with wikipedia
        /// </summary>
        [TestMethod]
        public void TestCrossProduct()
        {
            //check result: https://de.wikipedia.org/wiki/Kreuzprodukt
            Vector<double> v1 = DenseVector.OfArray(new[] { 1d, 2d, 3d });
            Vector<double> v2 = DenseVector.OfArray(new[] { -7d, 8d, 9d });
            Assert.AreEqual(MathUtil.CrossProduct(v1, v2), DenseVector.OfArray(new[] { -6d, -30d, 22d }));
        }

        /// <summary>
        /// Tests Metal Angles
        /// Checked wih Mercury
        /// </summary>
        [TestMethod]
        public void TestAngle()
        {
            var N1CrN4 = MathUtil.Angle(new List<Atom> { corrole.ByIdentifier("N1"), corrole.Metal, corrole.ByIdentifier("N4") });
            var N2CrN3 = MathUtil.Angle(new List<Atom> { corrole.ByIdentifier("N2"), corrole.Metal, corrole.ByIdentifier("N3") });
            //Tested with Mercury 4
            Assert.AreEqual(78.04, N1CrN4, 0.005);
            Assert.AreEqual(92.5, N2CrN3, 0.005);
        }

        /// <summary>
        /// Tests Dihedral Calculation
        /// </summary>
        [TestMethod]
        public void TestDihedral()
        {
            var result = new PorphyrinDihedrals(corrole.ByIdentifier).CalculateProperties().ToArray();

            //tested with Mercury
            var chi2 = -0.30;
            var chi1 = 14.51;
            var chi4 = -4.92;
            var chi3 = 0.01;
            var helicity = 1.68;

            Assert.AreEqual(chi2, Convert.ToDouble(result[0].Value.Trim('°')), 0.01);
            Assert.AreEqual(chi1, Convert.ToDouble(result[1].Value.Trim('°')), 0.01);
            Assert.AreEqual(chi4, Convert.ToDouble(result[2].Value.Trim('°')), 0.01);
            Assert.AreEqual(chi3, Convert.ToDouble(result[3].Value.Trim('°')), 0.01);

            result = new DefaultDihedrals(corrole.ByIdentifier).CalculateProperties().ToArray();
            Assert.AreEqual(helicity, Convert.ToDouble(result[0].Value.Trim('°')), 0.01);
        }

        /// <summary>
        /// Tests Interplanarangle
        /// </summary>
        [TestMethod]
        public void TestInterplanarAngle()
        {
            var result = new InterplanarAngle(corrole.ByIdentifier, corrole.Metal).CalculateProperties().ToArray();
            //tested with Mercury
            var interplanar = 46.22;
            Assert.AreEqual(interplanar, Convert.ToDouble(result[0].Value.Trim('°')), 0.01);
        }

        /// <summary>
        /// Checks validity of Mean Plane Method
        /// </summary>
        [TestMethod]
        public void TestPlane()
        {
            //tested with Mercury
            var a = -0.072;
            var b = -0.686;
            var c = 0.724;
            var d = 2.80762;

            var pl = corrole.GetMeanPlane();

            Assert.AreEqual(a, pl.A, 0.001);
            Assert.AreEqual(b, pl.B, 0.001);
            Assert.AreEqual(c, pl.C, 0.001);
            Assert.AreEqual(d, pl.D, 0.001);
        }

        /// <summary>
        /// Tests whether signed distance is correct
        /// </summary>
        [TestMethod]
        public void DistanceToMeanPlane()
        {
            //tested with mercury
            var d = 0.594;
            Assert.AreEqual(d, corrole.Metal.DistanceToPlane(corrole.GetMeanPlane()), 0.0005);
        }
    }
}
