using ExternalLib;

namespace StackExchange.Redis.Entity.Tests;

public static class Doc
{
    //public static class Fields
    //{
    //    public static readonly RedisValue EndDate = RedisEntity<Document>.Fields[nameof(Document.EndDate)];
    //    public static readonly RedisValue Price = RedisEntity<Document>.Fields[nameof(Document.Price)];
    //    public static readonly RedisValue IsDeleted = RedisEntity<Document>.Fields[nameof(Document.IsDeleted)];

    //    public static readonly RedisValue[] EndDate_IsDeleted = new[] { EndDate, IsDeleted };
    //}

    public static readonly RedisKey Key1 = "doc:0";
    public static readonly RedisKey Prefix = "doc:";

    public static readonly Document Data1 = new()
    {
        Id = Guid.NewGuid(),
        Name = "Самый важный документ для сдачи проекта 2015",
        StartDate = new DateOnly(2020, 04, 22),
        Price = 274_620_500,
        Size = DocumentSize.Medium,
        Created = DateTime.UtcNow
    };

    public static readonly ReadOnlyDocument ReadOnlyData1 = new(Data1);

    public static void New(Document doc, int i)
    {
        var random = Random.Shared;

        doc.Id = Guid.NewGuid();
        doc.Name = $"Самый важный документ для сдачи проекта №{i}";
        doc.StartDate = new DateOnly(random.Next(2000, 2024), random.Next(12), random.Next(28));
        doc.Price = random.NextInt64(1_000_000, 1_000_000_000);
        doc.Size = (DocumentSize)random.Next(0, 3);
        doc.Created = DateTime.UtcNow;
        doc.Modified = null;
        doc.EndDate = null;
        doc.IsDeleted = false;
    }
}