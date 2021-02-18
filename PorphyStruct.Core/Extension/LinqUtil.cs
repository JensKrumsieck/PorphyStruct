using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Core.Extension
{
    public static class LinqUtil
    {
        /// <summary>
        /// https://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (var s in list1)
            {
                if (cnt.ContainsKey(s)) cnt[s]++;
                else cnt.Add(s, 1);
            }
            foreach (var s in list2)
            {
                if (cnt.ContainsKey(s)) cnt[s]--;
                else return false;
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}
