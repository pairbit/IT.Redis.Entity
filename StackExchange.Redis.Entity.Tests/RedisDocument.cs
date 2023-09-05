using ExternalLib;

namespace StackExchange.Redis.Entity.Tests;

public class RedisDocument : IRedisEntityWriter<Document>, IRedisEntityReader<Document>
{
    private static readonly RedisValue[] _fields = new RedisValue[] { 0, 1, 2, 3, 4, 5 };

    public RedisValue[] Fields => _fields;

    public RedisValue Read(Document entity, RedisValue field)
    {
        var no = (int)field;

        if (no == 0) return entity.Name;
        if (no == 1) return entity.StartDate.DayNumber;
        if (no == 2) return entity.EndDate?.DayNumber;
        if (no == 3) return entity.Price;
        if (no == 4) return entity.IsDeleted;
        if (no == 5) return (int)entity.Size;

        throw new InvalidOperationException();
    }

    public void Write(Document entity, RedisValue field, RedisValue value)
    {
        var no = (int)field;

        if (no == 0) entity.Name = (string?)value;
        else if (no == 1) entity.StartDate = DateOnly.FromDayNumber((int)value);
        else if (no == 2) entity.EndDate = value.IsNull ? null : DateOnly.FromDayNumber((int)value);
        else if (no == 3) entity.Price = (long)value;
        else if (no == 4) entity.IsDeleted = (bool)value;
        else if (no == 5) entity.Size = (DocumentSize)(int)value;
        else throw new InvalidOperationException();
    }
}