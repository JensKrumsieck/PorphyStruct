using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Chemistry;

namespace PorphyStruct.Test
{
    [TestClass]
    public class AtomTests
    {
        /// <summary>
        /// Test Element Generation
        /// </summary>
        [TestMethod]
        public void TestElement() => Assert.AreEqual("S", new Atom("S10", 0, 0, 0).Type);
    }
}
