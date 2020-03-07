using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.Util
{
    /// <summary>
    /// A static class for operations on undirected graphs
    /// </summary>
    public static class DFSUtil
    {
        /// <summary>
        /// Implementation of recursive DFS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="func"></param>
        /// <param name="length</param>
        /// <returns></returns>
        public static async Task<HashSet<T>> DFS<T>(T vertex, Func<T, IEnumerable<T>> func)
        {
            var visited = new HashSet<T>();
            await Traverse(vertex, visited, func);
            return visited;
        }

        /// <summary>
        /// Traverse Function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="visited"></param>
        /// <param name="func"></param>
        /// <param name="length</param>
        public static async Task Traverse<T>(T vertex, HashSet<T> visited, Func<T, IEnumerable<T>> func)
        {
            visited.Add(vertex);
            foreach (var neighbor in func(vertex).Where(n => !visited.Contains(n)))
                await Traverse(neighbor, visited, func);
        }

        /// <summary>
        /// Returns all Paths from start to end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="func">Neighbor function</param>
        /// <param name="length">desired path length</param>
        /// <returns></returns>
        public static HashSet<HashSet<T>> GetAllPaths<T>(T start, T end, Func<T, IEnumerable<T>> func, int length = int.MaxValue)
        {
            var visited = new HashSet<T>();
            var localPaths = new HashSet<T>();
            var output = new HashSet<HashSet<T>>();
            output = AllPaths(start, end, visited, localPaths, func, output, length);
            return output;
        }

        /// <summary>
        /// Returns all paths
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="visited"></param>
        /// <param name="localPaths"></param>
        /// <param name="func"></param>
        /// <param name="output"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static HashSet<HashSet<T>> AllPaths<T>(T start, T end, HashSet<T> visited, HashSet<T> localPaths, Func<T, IEnumerable<T>> func, HashSet<HashSet<T>> output, int length = int.MaxValue)
        {
            visited.Add(start);
            if (localPaths.Count == 0) localPaths.Add(start);
            if (start.Equals(end) && localPaths.Count() == length) BuildPath(localPaths, output);

            foreach (var node in func(start))
            {
                if (!visited.Contains(node))
                {
                    localPaths.Add(node);
                    AllPaths(node, end, visited, localPaths, func, output, length);
                    localPaths.Remove(node);
                }
            }
            visited.Remove(start);
            return output;
        }

        /// <summary>
        /// Copys the localpath to output hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="localPath"></param>
        /// <param name="output"></param>
        private static void BuildPath<T>(HashSet<T> localPath, HashSet<HashSet<T>> output)
        {
            var BuildPath = new HashSet<T>();
            foreach (var node in localPath) BuildPath.Add(node);
            output.Add(BuildPath);
        }

        /// <summary>
        /// Calculates the vertex degree (count) of vertex in collection by func(vertex,collection)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="vertex"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static int VertexDegree<T>(IEnumerable<T> collection, T vertex, Func<T, IEnumerable<T>, IEnumerable<T>> func) => func(vertex, collection).Count();
    }
}
