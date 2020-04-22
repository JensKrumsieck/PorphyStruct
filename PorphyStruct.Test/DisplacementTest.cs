using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PorphyStruct.Chemistry;
using PorphyStruct.Simulations;
using System;
using System.Linq;
using System.Reflection;

namespace PorphyStruct.Test
{
    [TestClass]
    public class DisplacementTest
    {
        /// <summary>
        /// Tests whether the length of all Displacementvectors is correct
        /// </summary>
        [TestMethod]
        public void TestDisplacementLengths()
        {
            //♥LINQ
            foreach ((Macrocycle tmp, FieldInfo prop) in from type in (Macrocycle.Type[])Enum.GetValues(typeof(Macrocycle.Type))//build dummy cycle
                                                         let tmp = MacrocycleSetup.CreateWithDummyAtom(type)
                                                         let props = typeof(Displacements).GetFields()
                                                         from prop in props.Where(s => s.Name.Contains(type.ToString()))
                                                         select (tmp, prop))
            {
                //Porphyrins contain C20 twice!
                if (tmp.type == Macrocycle.Type.Porphyrin) Assert.AreEqual(tmp.RingAtoms.Count + 1, ((Vector<double>)prop.GetValue(null)).Count, 0, $"The Length of {prop.Name} is invalid");
                else
                    Assert.AreEqual(tmp.RingAtoms.Count, ((Vector<double>)prop.GetValue(null)).Count, 0, $"The Length of {prop.Name} is invalid");
            }
        }
    }
}
