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
            foreach (T item in list)
            {
                if (length == 1) yield return new T[] { item };
                else foreach (IEnumerable<T> result in GetCombinations(list.Skip(i + 1), length - 1)) yield return new T[] { item }.Concat(result);
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
            int i = collection.Count();
            while (--i >= 0)
            {
                T element = collection.ElementAt(i);
                if (predicate(element))
                {
                    collection.Remove(element);
                }
            }
        }

        /// <summary>
        /// Sorts an observable collection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null)
        {
            TSource[] sortedList;//Ein Array, damit die Elemente durch source.Clear nicht gelöscht werden
            if (comparer == null)
                sortedList = source.OrderBy(keySelector).ToArray();
            else
                sortedList = source.OrderBy(keySelector, comparer).ToArray();
            source.Clear();
            foreach (TSource item in sortedList)
                source.Add(item);
        }
    }
}