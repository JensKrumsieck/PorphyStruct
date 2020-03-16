using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PorphyStruct.Util
{
    public static class LinqUtil
    {
        /// <summary>
        /// Gets All Combinations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes all items in observable collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            var i = collection.Count();
            while (--i >= 0)
            {
                var element = collection.ElementAt(i);
                if (predicate(element))
                {
                    collection.Remove(element);
                }
            }
        }
    }
}