using System.Collections;
using System.Numerics;

namespace DocLib;

public interface IReadOnlyDocument
{
    Guid Id { get; }

    Guid ExternalId { get; }

    string? Name { get; }

    char Character { get; }

#if NET6_0_OR_GREATER

    DateOnly StartDate { get; }

    DateOnly? EndDate { get; }

#endif

    long Price { get; }

    bool IsDeleted { get; }

    DocumentSize Size { get; }

    DateTime Created { get; }

    DateTime? Modified { get; }

    byte[]? Content { get; }

    ReadOnlyMemory<byte>? MemoryBytes { get; }

    BigInteger BigInteger { get; }

    BitArray? Bits { get; }

    Version? Version { get; }

    Uri? Url { get; }

    int[]? IntArray { get; }

    int?[]? IntArrayN { get; }

    IReadOnlyList<Guid?>? TagIds { get; }

    DocumentVersionInfo? VersionInfo { get; }

    DocumentVersionInfos? VersionInfos { get; }

    IEnumerable<decimal?>? Decimals { get; }

    IEnumerable<char>? Chars { get; }
}