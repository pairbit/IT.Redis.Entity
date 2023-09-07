using StackExchange.Redis;
using StackExchange.Redis.Entity;
using StackExchange.Redis.Entity.Serializers;
using System.Linq.Expressions;

namespace DocLib.RedisEntity;

public class RedisDocumentArrayExpression : IRedisEntity<Document>
{
    private static readonly IRedisValueDeserializer _guidBytesDeserializer = new GuidBytesSerializer();
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

    private readonly Func<Document, RedisValue>[] _readers = new Func<Document, RedisValue>[9];
    private readonly Action<Document, RedisValue>[] _writers = new Action<Document, RedisValue>[9];

    public RedisDocumentArrayExpression()
    {
        _readers[0] = Compile(x => x.Name);
        _writers[0] = Compile(x => x.Name, v => (string?)v);

        _readers[1] = Compile(x => x.StartDate.DayNumber);
        _writers[1] = Compile(x => x.StartDate, v => DateOnly.FromDayNumber((int)v));

        _readers[2] = Compile(x => x.EndDate == null ? RedisValue.EmptyString : x.EndDate.Value.DayNumber);
        _writers[2] = Compile(x => x.EndDate, v => v.IsNullOrEmpty ? null : DateOnly.FromDayNumber((int)v));

        _readers[3] = Compile(x => x.Price);
        _writers[3] = Compile(x => x.Price, v => (long)v);

        _readers[4] = Compile(x => x.IsDeleted);
        _writers[4] = Compile(x => x.IsDeleted, v => (bool)v);

        _readers[5] = Compile(x => (int)x.Size);
        _writers[5] = Compile(x => x.Size, v => (DocumentSize)(int)v);

        _readers[6] = Compile(x => x.Created.Ticks);
        _writers[6] = Compile(x => x.Created, v => new DateTime((long)v));

        _readers[7] = Compile(x => x.Modified == null ? RedisValue.EmptyString : x.Modified.Value.Ticks);
        _writers[7] = Compile(x => x.Modified, v => v.IsNullOrEmpty ? null : new DateTime((long)v));

        _readers[8] = Compile(x => x.Id.ToByteArray());
        _writers[8] = Compile(x => x.Id, v => ((IRedisValueDeserializer<Guid>)_guidBytesDeserializer).Deserialize(in v));
    }

    private static Func<Document, RedisValue> Compile(Expression<Func<Document, RedisValue>> reader) => reader.Compile();

    private static Action<Document, RedisValue> Compile<T>(Expression<Func<Document, T>> setter, Expression<Func<RedisValue, T>> getValue)
    {
        var set = MakeSet(setter).Compile();
        var value = getValue.Compile();
        return (doc, redisValue) => set(doc, value(redisValue));
    }

    private static void UpdateFeature<T, TResult>(T feature, Expression<Func<T, TResult>> e, TResult newValue)
    {
        var x = e.Parameters[0];
        var exp = Expression.Assign(e.Body, Expression.Constant(newValue));
        var lambda = Expression.Lambda<Action<T>>(exp, x);
        lambda.Compile()(feature);
    }

    private static Expression<Action<M, R>> MakeSet<M, R>(Expression<Func<M, R>> fetcherExp)
    {
        if (fetcherExp.Body.NodeType != ExpressionType.MemberAccess)
        {
            throw new ArgumentException(
                "This should be a member getter",
                "fetcherExp");
        }

        //    Input model
        var model = fetcherExp.Parameters[0];
        //    Input value to set
        var value = Expression.Variable(typeof(R), "v");
        //    Member access
        var member = fetcherExp.Body;
        //    We turn the access into an assignation to the input value
        var assignation = Expression.Assign(member, value);
        //    We wrap the action into a lambda expression with parameters
        var assignLambda = Expression.Lambda<Action<M, R>>(assignation, model, value);

        return assignLambda;
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