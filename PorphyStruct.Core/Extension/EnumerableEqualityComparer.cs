namespace PorphyStruct.Core.Extension;

public sealed class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>> where T : notnull, IEquatable<T>
{
    public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.ScrambledEquals(y);
    }

    public int GetHashCode(IEnumerable<T> obj) => 0;
}
