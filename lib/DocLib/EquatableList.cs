namespace DocLib;

public class EquatableList<T> : List<T>, IEquatable<EquatableList<T>>
{
    private readonly IEqualityComparer<T>? _comparer;

    public EquatableList(IEqualityComparer<T>? comparer = null)
    {
        _comparer = comparer;
    }

    public EquatableList(int capacity, IEqualityComparer<T>? comparer = null) : base(capacity)
    {
        _comparer = comparer;
    }

    public override bool Equals(object? obj) => Equals(obj as EquatableList<T>);

    public bool Equals(EquatableList<T>? other) => ReferenceEquals(this, other) || other is not null && this.SequenceEqual(other, _comparer);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        for (int i = 0; i < Count; i++)
        {
            hash.Add(this[i], _comparer);
        }
        return hash.ToHashCode();
    }
}