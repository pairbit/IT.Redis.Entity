using StackExchange.Redis;
using StackExchange.Redis.Entity.Attributes;
using System.Collections;
using System.Numerics;

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

    public DocumentSize? SizeNullable { get; set; }

    //public Enum SizeEnum { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }
    
    [RedisField(9)]
    public byte[]? Content { get; set; }

    [RedisField("mem")]
    public ReadOnlyMemory<byte> MemoryBytes { get; set; }

    [RedisField("mem-null")]
    public ReadOnlyMemory<byte>? MemoryBytesNull { get; set; }

    public RedisValue RedisValNull { get; set; }

    public RedisValue? RedisValNullable { get; set; }

    public RedisValue RedisValEmpty { get; set; }

    public RedisValue RedisValNum { get; set; }

    public IntPtr IntPtrValue { get; set; }

    public BigInteger BigInteger { get; set; }

    //[RedisFieldIgnore]
    public UIntPtr UIntPtrValue { get; set; }

    public BitArray Bits { get; set; }

    public int[] Nums { get; set; }

    public ICollection<int> NumCollection { get; set; }

    public static void New(DocumentDepend doc, int i)
    {
        var random = Random.Shared;

        doc.Id = Guid.NewGuid();
        doc.ClientId = Guid.NewGuid();
        doc.Name = null;// $"Самый важный документ для сдачи проекта №{i}";
        doc.StartDate = new DateOnly(random.Next(2000, 2024), random.Next(1, 13), random.Next(1, 29));
        doc.Price = random.NextInt64(1_000_000, 1_000_000_000);
        doc.Size = (DocumentSize)random.Next(0, 3);
        //doc.SizeEnum = doc.Size;
        doc.Created = DateTime.UtcNow;
        doc.Modified = null;
        doc.EndDate = null;
        doc.IsDeleted = false;
        doc.Character = char.MaxValue;

        doc.Content = Array.Empty<byte>();

        var content = new byte[1024];
        random.NextBytes(content);
        doc.MemoryBytes = content;
        doc.MemoryBytesNull = null;

        doc.RedisValNull = RedisValue.Null;
        doc.RedisValNullable = null;
        doc.RedisValEmpty = RedisValue.EmptyString;
        doc.RedisValNum = random.NextInt64();

        doc.IntPtrValue = (IntPtr)random.NextInt64();
        doc.UIntPtrValue = (UIntPtr)random.NextInt64();

        var bytes = new byte[32];
        random.NextBytes(bytes);
        doc.BigInteger = new BigInteger(bytes);
        doc.Bits = new BitArray(new bool[] { true, false, true, false, false, true, true, true });
        doc.Nums = new int[] { 12, 9, 122, 999, 0, 2 };
        doc.NumCollection = new int[] { 2, 4, 43, 1, 40, 453, 88, 3 };
    }
}