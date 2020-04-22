using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Chemistry;
using PorphyStruct.Chemistry.Macrocycles;
using PorphyStruct.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Test
{
    [TestClass]
    public class DFSTest
    {
        private Corrole Corrole { get; set; } = MacrocycleSetup.CreateTestCorroleCIF();


        /// <summary>
        /// Tests DFS to find only one connected figure with more atoms than ringatoms
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDFS()
        {
            var figures = new List<IEnumerable<Atom>>();
            await foreach (IEnumerable<Atom> fig in DFSUtil.ConnectedFigures(Corrole.Atoms.Where(s => !s.IsMetal && Corrole.NonMetalNeighbors(s).Count() >= 2), Corrole.NonMetalNeighbors))
                figures.Add(fig);
            Assert.IsTrue(figures.Where(s => s.Count() >= Corrole.RingAtoms.Count()).Count() == 1);
        }

        /// <summary>
        /// Makes sure that corrole is still valid after detect
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestDetection()
        {
            await Corrole.Detect();
            Assert.IsTrue(Corrole.IsValid);
            //if the detect runs completely only the macrocylic atoms have no X appended to identifier!
            Assert.IsTrue(Corrole.Atoms.Count(s => !s.Identifier.Contains("X")) == Corrole.RingAtoms.Count);
        }
    }
}
