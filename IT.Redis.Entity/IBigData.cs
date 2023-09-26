namespace IT.Redis.Entity;

internal interface IBigData
{
    int Save(ReadOnlySpan<byte> bigData, bool isCompressed, Span<byte> externalId);

    byte[] Load(ReadOnlySpan<byte> externalId);
}