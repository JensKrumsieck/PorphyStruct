using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Util
{
    public static class LinqUtil
    {
        public static IEnumerable<IEnumerable<T>> GetCombinations<T>(this IEnumerable<T> list, int length)
        {
            int i = 0;
            foreach (var item in list)
            {
                if (length == 1) yield return new T[] { item };
                else foreach (var result in GetCombinations(list.Skip(i + 1), length - 1)) yield return new T[] { item }.Concat(result);
                ++i;
            }
        }
    }
}
