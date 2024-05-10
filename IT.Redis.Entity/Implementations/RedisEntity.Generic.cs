using System.Reflection;

namespace IT.Redis.Entity;

public class RedisEntity<TEntity>
{
    internal static readonly PropertyInfo[] Properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static Lazy<IRedisEntity<TEntity>> _default = new(RedisEntity.Factory.New<TEntity>);

    public static Func<IRedisEntity<TEntity>> Factory
    {
        set
        {
            _default = new(value ?? throw new ArgumentNullException(nameof(value)));
        }
    }

    public static IRedisEntity<TEntity> Default => _default.Value;
}