using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Chemistry;

namespace PorphyStruct.Test
{
    [TestClass]
    public class AtomTests
    {
        private Atom Dummy { get; set; } = new Atom("Co12345", 42, 42, 42);
        private Atom Phosphorus { get; set; } = new Atom("P1", 0, 0, 0);

        /// <summary>
        /// Test Element Generation
        /// </summary>
        [TestMethod]
        public void TestElement() => Assert.AreEqual("Co", Dummy.Type);

        /// <summary>
        /// Tests Vector-Array-Conversion
        /// </summary>
        [TestMethod]
        public void TestVector() => CollectionAssert.AreEqual(Dummy.XYZ, Dummy.Vector.ToArray());

        /// <summary>
        /// 72.746134 is distance between root and 42,42,42.
        /// </summary>
        [TestMethod]
        public void TestDistance() => Assert.AreEqual(72.746134, Atom.Distance(Dummy, Phosphorus), 0.00001);

        /// <summary>
        /// Tests Metal Status of Cobalt ;)
        /// </summary>
        [TestMethod]
        public void TestMetal() => Assert.IsTrue(Dummy.IsMetal);

        /// <summary>
        /// Check whether definition is met
        /// </summary>
        [TestMethod]
        public void TestPhosphorus() => Assert.IsTrue(Phosphorus.IsMetal && Core.Properties.Settings.Default.ExtendMetalDefinition);
    }
}
