using System.Collections;
using System.Numerics;

namespace DocLib;

public interface IDocument : IReadOnlyDocument
{
    byte[]? RedisKey { get; set; }

    byte RedisKeyBits { get; set; }

    new Guid Id { get; set; }

    new Guid ExternalId { get; set; }

    new string? Name { get; set; }

    new char Character { get; set; }

#if NET6_0_OR_GREATER

    new DateOnly StartDate { get; set; }

    new DateOnly? EndDate { get; set; }

#endif

    new long Price { get; set; }

    new bool IsDeleted { get; set; }

    new DocumentSize Size { get; set; }

    new DateTime Created { get; set; }

    new DateTime? Modified { get; set; }

    new byte[]? Content { get; set; }

    new ReadOnlyMemory<byte>? MemoryBytes { get; set; }

    new BigInteger BigInteger { get; set; }

    new BitArray? Bits { get; set; }

    new Version? Version { get; set; }

    new Uri? Url { get; set; }

    new int[]? IntArray { get; set; }

    new int?[]? IntArrayN { get; set; }

    new List<Guid?>? TagIds { get; set; }

    new DocumentVersionInfo? VersionInfo { get; set; }

    new DocumentVersionInfos? VersionInfos { get; set; }

    new IEnumerable<decimal?>? Decimals { get; set; }

    new IEnumerable<char>? Chars { get; set; }
}