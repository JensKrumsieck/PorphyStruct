using System.Threading.Tasks.Dataflow;

namespace PorphyStruct.Core.Extension;

public static class LinqUtil
{
    /// <summary>
    /// https://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <returns></returns>
    public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2) where T : notnull
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

    /// <summary>
    /// Returns source without last element
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="src"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> RemoveLast<TSource>(this IEnumerable<TSource> src)
    {
        var list = src.ToList();
        return list.Take(list.Count - 1);
    }

    /// <summary>
    /// Based on https://medium.com/@alex.puiu/parallel-foreach-async-in-c-36756f8ebe62
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="body"></param>
    /// <param name="maxDegreeOfParallelism"></param>
    /// <returns></returns>
    public static async Task AsyncParallelForeach<T>(this IEnumerable<T> src, Action<T> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded)
    {
        var options = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };
        var block = new ActionBlock<T>(body, options);
        foreach (var item in src) block.Post(item);
        block.Complete();
        await block.Completion;
    }
}
