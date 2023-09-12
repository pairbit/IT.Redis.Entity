using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DocLib;

public class DocumentEqualityComparer : IEqualityComparer<IReadOnlyDocument>
{
    public static readonly DocumentEqualityComparer Default = new();

    public bool Equals(IReadOnlyDocument? x, IReadOnlyDocument? y)
        => ReferenceEquals(x, y) || (
        x != null && y != null &&
        x.Id == y.Id &&
        x.ExternalId == y.ExternalId &&
        x.Name == y.Name &&
        x.Modified == y.Modified &&
        x.BigInteger == y.BigInteger &&
        x.Size == y.Size &&
        x.Price == y.Price &&
        x.Character == y.Character &&
        Seq(x.Content, y.Content) &&
        Seq(x.MemoryBytes, y.MemoryBytes) &&
        x.StartDate == y.StartDate &&
        x.EndDate == y.EndDate &&
        x.IsDeleted == y.IsDeleted &&
        x.Created == y.Created &&
        Eq(x.Url, y.Url) &&
        Eq(x.Version, y.Version) &&
        Seq<bool>(x.Bits, y.Bits) &&
        Seq(x.IntArray, y.IntArray) &&
        Seq(x.IntArrayN, y.IntArrayN) &&
        Seq(x.TagIds, y.TagIds)
        );

    public int GetHashCode([DisallowNull] IReadOnlyDocument obj)
    {
        throw new NotImplementedException();
    }

    private static bool Eq<T>(T? x, T? y) => ReferenceEquals(x, y) || (x != null && y != null && x.Equals(y));

    private static bool Seq<T>(IEnumerable<T>? x, IEnumerable<T>? y) => ReferenceEquals(x, y) || (x != null && y != null && x.SequenceEqual(y));

    private static bool Seq<T>(ReadOnlyMemory<T>? x, ReadOnlyMemory<T>? y) => (x == null && y == null) || (x != null && y != null && x.Value.Span.SequenceEqual(y.Value.Span));

    private static bool Seq<T>(IEnumerable? x, IEnumerable? y) => ReferenceEquals(x, y) || (x != null && y != null && x.Cast<T>().SequenceEqual(y.Cast<T>()));
}