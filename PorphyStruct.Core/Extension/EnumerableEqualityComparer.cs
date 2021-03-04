using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Extension
{
    public class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>> where T : IEquatable<T>
    {
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y) => x.ScrambledEquals(y);

        public int GetHashCode(IEnumerable<T> obj) => 0;
    }
}
