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
        (ReferenceEquals(x.Content, y.Content) || (x.Content != null && y.Content != null && x.Content.SequenceEqual(y.Content))) &&
        ((x.MemoryBytes == null && y.MemoryBytes == null) || (x.MemoryBytes != null && y.MemoryBytes != null && x.MemoryBytes.Value.Span.SequenceEqual(y.MemoryBytes.Value.Span))) &&
        x.StartDate == y.StartDate &&
        x.EndDate == y.EndDate &&
        x.IsDeleted == y.IsDeleted &&
        x.Created == y.Created
        );

    public int GetHashCode([DisallowNull] IReadOnlyDocument obj)
    {
        throw new NotImplementedException();
    }
}