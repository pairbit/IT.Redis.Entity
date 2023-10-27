using StackExchange.Redis;
using IT.Redis.Entity;
using IT.Redis.Entity.Formatters;

namespace DocLib.RedisEntity;

public class RedisDocument : IRedisEntityReaderWriter<Document>
{
    private static readonly IRedisValueFormatter<DocumentSize> DocumentSizeFormatter = new EnumFormatter<DocumentSize, sbyte>();

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

    public IKeyBuilder KeyBuilder => throw new NotImplementedException();

    public IRedisValueDeserializer<TField> GetDeserializer<TField>(in RedisValue field) => (IRedisValueDeserializer<TField>)GetFormatter((int)field);

    public IRedisValueSerializer<TField> GetSerializer<TField>(in RedisValue field) => (IRedisValueSerializer<TField>)GetFormatter((int)field);

    public RedisValue Read(Document entity, in RedisValue field)
    {
        var no = (int)field;

        if (no == 0) return entity.Name;
#if NET6_0_OR_GREATER
        if (no == 1) return entity.StartDate.DayNumber;
        if (no == 2) return entity.EndDate?.DayNumber ?? RedisValue.EmptyString;
#endif
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

        if (no == 0) entity.Name = (string)value!;
#if NET6_0_OR_GREATER
        else if (no == 1) entity.StartDate = DateOnly.FromDayNumber((int)value);
        else if (no == 2) entity.EndDate = value.IsNullOrEmpty ? null : DateOnly.FromDayNumber((int)value);
#endif
        else if (no == 3) entity.Price = (long)value;
        else if (no == 4) entity.IsDeleted = (bool)value;
        else if (no == 5) entity.Size = (DocumentSize)(int)value;
        else if (no == 6) entity.Created = new DateTime((long)value);
        else if (no == 7) entity.Modified = value.IsNullOrEmpty ? null : new DateTime((long)value);
        else if (no == 8) entity.Id = new Guid(((byte[])value)!);
        else throw new InvalidOperationException();

        return true;
    }

    internal static object GetFormatter(int no)
    {
        if (no == 0) return StringFormatter.Default;
#if NET6_0_OR_GREATER
        if (no == 1) return DateOnlyFormatter.Default;
        if (no == 2) return DateOnlyFormatter.Default;
#endif
        if (no == 3) return Int64Formatter.Default;
        if (no == 4) return BooleanFormatter.Default;
        if (no == 5) return DocumentSizeFormatter;
        if (no == 6) return DateTimeFormatter.Default;
        if (no == 7) return DateTimeFormatter.Default;
        if (no == 8) return GuidFormatter.Default;

        throw new InvalidOperationException();
    }

    public RedisKey ReadKey(Document entity)
    {
        throw new NotImplementedException();
    }
}