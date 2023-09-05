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

    public static readonly RedisKey Key1 = "doc:1";

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
}