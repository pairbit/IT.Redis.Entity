using StackExchange.Redis;
using StackExchange.Redis.Entity;

namespace DocLib.RedisEntity;

public delegate RedisValue RedisValueReader(Document entity);
public delegate void RedisValueWriter(Document entity, RedisValue value);

public class RedisDocumentReaderWriterArray : IRedisEntityWriter<Document>, IRedisEntityReader<Document>
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

    private readonly RedisValueReader[] _readers = new RedisValueReader[9];
    private readonly RedisValueWriter[] _writers = new RedisValueWriter[9];

    public RedisDocumentReaderWriterArray()
    {
        _readers[0] = x => x.Name;
        _writers[0] = (x, v) => x.Name = v;

        _readers[1] = x => x.StartDate.DayNumber;
        _writers[1] = (x, v) => x.StartDate = DateOnly.FromDayNumber((int)v);

        _readers[2] = x => x.EndDate?.DayNumber ?? RedisValue.EmptyString;
        _writers[2] = (x, v) => x.EndDate = v.IsNullOrEmpty ? null : DateOnly.FromDayNumber((int)v);

        _readers[3] = x => x.Price;
        _writers[3] = (x, v) => x.Price = (long)v;

        _readers[4] = x => x.IsDeleted;
        _writers[4] = (x, v) => x.IsDeleted = (bool)v;

        _readers[5] = x => (int)x.Size;
        _writers[5] = (x, v) => x.Size = (DocumentSize)(int)v;

        _readers[6] = x => x.Created.Ticks;
        _writers[6] = (x, v) => x.Created = new DateTime((long)v);

        _readers[7] = x => x.Modified?.Ticks ?? RedisValue.EmptyString;
        _writers[7] = (x, v) => x.Modified = v.IsNullOrEmpty ? null : new DateTime((long)v);

        _readers[8] = x => x.Id.ToByteArray();
        _writers[8] = (x, v) => x.Id = new Guid(((ReadOnlyMemory<byte>)v).Span);
    }

    public RedisValue Read(Document entity, in RedisValue field)
    {
        var i = (int)field;

        if (i < 0 || i > _readers.Length) throw new ArgumentOutOfRangeException(nameof(field));

        return _readers[i](entity);
    }

    public bool Write(Document entity, in RedisValue field, in RedisValue value)
    {
        if (value.IsNull) return false;

        var i = (int)field;

        if (i < 0 || i > _writers.Length) throw new ArgumentOutOfRangeException(nameof(field));

        _writers[i](entity, value);

        return true;
    }
}