using IT.Redis.Entity.Attributes;
using IT.Redis.Entity.Internal;
using IT.Redis.Entity.Utf8Formatters;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class AnnotationConfiguration : IRedisEntityConfiguration
{
    private readonly IRedisValueFormatter _formatter = RedisValueFormatterNotRegistered.Default;

    public AnnotationConfiguration() { }

    public AnnotationConfiguration(IRedisValueFormatter formatter)
    {
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    public virtual IRedisValueFormatter GetFormatter(PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<RedisValueFormatterAttribute>(true);

        if (attr != null)
        {
            var formatter = attr.GetFormatterObject() ?? throw new InvalidOperationException();

            var formatterPropertyType = typeof(IRedisValueFormatter<>).MakeGenericType(property.PropertyType);

            if (!formatterPropertyType.IsAssignableFrom(formatter.GetType()))
                throw new InvalidOperationException();

            return new RedisValueFormatterProxy(formatter);
        }

        return _formatter;
    }

    public virtual string? GetKeyPrefix(Type type)
        => type.GetCustomAttribute<RedisKeyPrefixAttribute>()?.Prefix;

    public BindingFlags GetBindingFlags(Type type)
        => BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public IKeyRebuilder? GetKeyBuilder(Type type) => null;

    public KeyReader<TEntity>? GetKeyReader<TEntity>() => null;

    public virtual object GetUtf8Formatter(PropertyInfo property)
        => Utf8FormatterVar.GetFormatter(property.PropertyType) ?? throw Ex.Utf8FormatterNotFound(property.PropertyType);

    public RedisValueWriter<TEntity>? GetWriter<TEntity>(PropertyInfo property) => null;

    public RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property) => null;

    public virtual bool IsKey(PropertyInfo property) => property.GetCustomAttribute<RedisKeyAttribute>() != null;

    public virtual bool IsIgnore(PropertyInfo property) => property.GetCustomAttribute<RedisFieldIgnoreAttribute>() != null;

    public virtual bool TryGetField(PropertyInfo property, out RedisValue field)
    {
        var redisField = property.GetCustomAttribute<RedisFieldAttribute>();
        if (redisField != null)
        {
            if (redisField.Id >= 0)
            {
                field = redisField.Id;
                return true;
            }
            if (redisField.Name != null)
            {
                field = redisField.Name;
                return true;
            }
        }

        if (property.Name != Compiler.PropNameRedisKey &&
            property.Name != Compiler.PropNameRedisKeyBits &&
            (property.GetMethod?.IsPublic == true ||
            property.SetMethod?.IsPublic == true))
        {
            field = property.Name;
            return true;
        }

        field = RedisValue.Null;
        return false;
    }
}