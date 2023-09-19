using StackExchange.Redis;

namespace DocLib.RedisEntity;

public class DocumentNotNullable
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public char Character { get; set; }

#if NET6_0_OR_GREATER
    public DateOnly Date { get; set; }
#endif

    public long Price { get; set; }

    public bool IsDeleted { get; set; }

    public DocumentSize Size { get; set; }

    public byte[] Content { get; set; }

    public ReadOnlyMemory<byte> Memory { get; set; }

    public RedisValue RedisVal { get; set; }

    public IntPtr IntPtrValue { get; set; }
}