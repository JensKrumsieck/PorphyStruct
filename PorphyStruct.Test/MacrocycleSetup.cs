using PorphyStruct.Chemistry;
using PorphyStruct.Chemistry.Macrocycles;
using PorphyStruct.Core.Util;
using PorphyStruct.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PorphyStruct.Test
{
    internal static class MacrocycleSetup
    {
        /// <summary>
        /// Builds a dummy macrocycle with dummy atom to test instance related properties
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Macrocycle CreateWithDummyAtom(Macrocycle.Type type) => MacrocycleFactory.Build(new AsyncObservableCollection<Atom>(new List<Atom>() { new Atom("Co42", 0, 0, 0)
        }), type);

        /// <summary>
        /// test Cr-Corrole from DOI: 10.1142/S1088424619500792
        /// </summary>
        /// <returns></returns>
        internal static Corrole CreateTestCorroleMol2()
        {
            var x = ReadResource("testcorrole.mol2");
            var file = new Mol2File
            {
                Content = x,
                Lines = x.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None)
            };

            return (Corrole)MacrocycleFactory.Build(file.GetMolecule().Atoms, Macrocycle.Type.Corrole);
        }

        /// <summary>
        /// test Cr-Corrole from DOI: 10.1142/S1088424619500792
        /// </summary>
        /// <returns></returns>
        internal static Corrole CreateTestCorroleCIF()
        {
            var x = ReadResource("testcorrole.cif");
            var file = new CifFile
            {
                Content = x,
                Lines = x.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None)
            };

            return (Corrole)MacrocycleFactory.Build(file.GetMolecule().Atoms, Macrocycle.Type.Corrole);
        }

        /// <summary>
        /// reads in Resource files
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ReadResource(string name)
        {
            var rs = Assembly.GetCallingAssembly()
                .GetManifestResourceStream($"PorphyStruct.Test.Resources.{name}");
            using var reader = new StreamReader(rs, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
