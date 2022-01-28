using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Core.Analysis;

namespace PorphyStruct.Test
{
    [TestClass]
    public class DetectionTest
    {
        /// <summary>
        /// Tests Detection capability and speed on a cif with 4 cobalt corroles in it
        /// Used CCDC Identifier: 830942 (OCISOK)
        /// G.Pomarico, F.R.Fronczek, S.Nardis, K.M.Smith, R.Paolesse, Journal of Porphyrins and Phthalocyanines, 2011, 15, 1085,
        /// DOI: 10.1142/S1088424611004038 
        /// </summary>
        [TestMethod]
        public async Task DetectCorroles() =>
            await CommonTestMethods.RunDetection("testfiles/830942.cif", MacrocycleType.Corrole, 4);
    }
}
