using StackExchange.Redis;
using StackExchange.Redis.Entity;

namespace DocLib.RedisEntity;

public class RedisDocument : IRedisEntityReaderWriter<Document>
{
    private static readonly IRedisEntityFields _fields = new RedisEntityFields(new Dictionary<string, RedisValue>
    {
        { nameof(Document.Name), 0 },
        { nameof(Document.StartDate), 1 },
        { nameof(Document.EndDate), 2 },
        { nameof(Document.Price), 3 },
        { nameof(Document.IsDeleted), 4 },
        { nameof(Document.Size), 5 },
        { nameof(Document.Created), 6 },
        { nameof(Document.Modified), 7 },
        { nameof(Document.Id), 8 },
    });

    public IRedisEntityFields Fields => _fields;

    public RedisValue Read(Document entity, in RedisValue field)
    {
        var no = (int)field;

        if (no == 0) return entity.Name;
        if (no == 1) return entity.StartDate.DayNumber;
        if (no == 2) return entity.EndDate?.DayNumber ?? RedisValue.EmptyString;
        if (no == 3) return entity.Price;
        if (no == 4) return entity.IsDeleted;
        if (no == 5) return (int)entity.Size;
        if (no == 6) return entity.Created.Ticks;
        if (no == 7) return entity.Modified?.Ticks ?? RedisValue.EmptyString;
        if (no == 8) return entity.Id.ToByteArray();

        throw new InvalidOperationException();
    }

    public bool Write(Document entity, in RedisValue field, in RedisValue value)
    {
        if (value.IsNull) return false;
        var no = (int)field;

        if (no == 0) entity.Name = (string?)value;
        else if (no == 1) entity.StartDate = DateOnly.FromDayNumber((int)value);
        else if (no == 2) entity.EndDate = value.IsNullOrEmpty ? null : DateOnly.FromDayNumber((int)value);
        else if (no == 3) entity.Price = (long)value;
        else if (no == 4) entity.IsDeleted = (bool)value;
        else if (no == 5) entity.Size = (DocumentSize)(int)value;
        else if (no == 6) entity.Created = new DateTime((long)value);
        else if (no == 7) entity.Modified = value.IsNullOrEmpty ? null : new DateTime((long)value);
        else if (no == 8) entity.Id = new Guid(((ReadOnlyMemory<byte>)value).Span);
        else throw new InvalidOperationException();

        return true;
    }
}