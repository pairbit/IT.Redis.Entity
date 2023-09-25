using StackExchange.Redis;
using IT.Redis.Entity;

namespace DocLib.RedisEntity;

public class RedisDocumentArray : IRedisEntityReaderWriter<Document>
{
    private static readonly IRedisEntityFields _fields = new RedisEntityFields(new Dictionary<string, RedisValue>
    {
        { nameof(Document.Name), 0 },
#if NET6_0_OR_GREATER
        { nameof(Document.StartDate), 1 },
        { nameof(Document.EndDate), 2 },
#endif
        { nameof(Document.Price), 3 },
        { nameof(Document.IsDeleted), 4 },
        { nameof(Document.Size), 5 },
        { nameof(Document.Created), 6 },
        { nameof(Document.Modified), 7 },
        { nameof(Document.Id), 8 },
    });

    public IRedisEntityFields Fields => _fields;

    private readonly Func<Document, RedisValue>[] _readers = new Func<Document, RedisValue>[9];
    private readonly Action<Document, RedisValue>[] _writers = new Action<Document, RedisValue>[9];

    public RedisDocumentArray()
    {
        _readers[0] = x => x.Name;
        _writers[0] = (x, v) => x.Name = (string)v!;
#if NET6_0_OR_GREATER
        _readers[1] = x => x.StartDate.DayNumber;
        _writers[1] = (x, v) => x.StartDate = DateOnly.FromDayNumber((int)v);

        _readers[2] = x => x.EndDate?.DayNumber ?? RedisValue.EmptyString;
        _writers[2] = (x, v) => x.EndDate = v.IsNullOrEmpty ? null : DateOnly.FromDayNumber((int)v);
#endif
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
        _writers[8] = (x, v) => x.Id = new Guid((byte[])v!);
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

    public IRedisValueDeserializer<TField> GetDeserializer<TField>(in RedisValue field) 
        => (IRedisValueDeserializer<TField>)RedisDocument.GetFormatter((int)field);

    public IRedisValueSerializer<TField> GetSerializer<TField>(in RedisValue field) 
        => (IRedisValueSerializer<TField>)RedisDocument.GetFormatter((int)field);
}