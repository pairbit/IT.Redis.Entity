using StackExchange.Redis;

namespace DocLib.RedisEntity;

public class DocumentNullable
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public char? Character { get; set; }

    public DateOnly? Date { get; set; }

    public long? Price { get; set; }

    public bool? IsDeleted { get; set; }

    public DocumentSize? Size { get; set; }

    public byte[]? Content { get; set; }

    public ReadOnlyMemory<byte>? Memory { get; set; }

    public RedisValue? RedisVal { get; set; }

    public IntPtr? IntPtrValue { get; set; }
}