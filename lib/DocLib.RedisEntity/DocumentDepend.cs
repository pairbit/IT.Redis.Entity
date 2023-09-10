using StackExchange.Redis;
using StackExchange.Redis.Entity.Attributes;

namespace DocLib.RedisEntity;

public record DocumentDepend
{
    [GuidHexFormatter("D")]
    public Guid Id { get; set; }

    [GuidHexFormatter]
    public Guid ClientId { get; set; }

    public string? Name { get; set; }

    public char Character { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public long Price { get; set; }

    public bool IsDeleted { get; set; }

    public DocumentSize Size { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }
    
    [RedisField(9)]
    public byte[]? Content { get; set; }

    [RedisField("mem")]
    public ReadOnlyMemory<byte> MemoryBytes { get; set; }

    public RedisValue RedisValNull { get; set; }

    public RedisValue RedisValEmpty { get; set; }

    public RedisValue RedisValNum { get; set; }

    public IntPtr IntPtrValue { get; set; }

    //[RedisFieldIgnore]
    public UIntPtr UIntPtrValue { get; set; }

    public static void New(DocumentDepend doc, int i)
    {
        var random = Random.Shared;

        doc.Id = Guid.NewGuid();
        doc.ClientId = Guid.NewGuid();
        doc.Name = null;// $"Самый важный документ для сдачи проекта №{i}";
        doc.StartDate = new DateOnly(random.Next(2000, 2024), random.Next(1, 13), random.Next(1, 29));
        doc.Price = random.NextInt64(1_000_000, 1_000_000_000);
        doc.Size = (DocumentSize)random.Next(0, 3);
        doc.Created = DateTime.UtcNow;
        doc.Modified = null;
        doc.EndDate = null;
        doc.IsDeleted = false;
        doc.Character = char.MaxValue;

        doc.Content = new byte[] { 0 };

        var content = new byte[1024];
        random.NextBytes(content);
        doc.MemoryBytes = content;

        doc.RedisValNull = RedisValue.Null;
        doc.RedisValEmpty = RedisValue.EmptyString;
        doc.RedisValNum = random.NextInt64();

        doc.IntPtrValue = (IntPtr)random.NextInt64();
        doc.UIntPtrValue = (UIntPtr)random.NextInt64();
    }
}