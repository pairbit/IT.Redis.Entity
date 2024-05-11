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

    public virtual RedisValue GetField(PropertyInfo property, out bool hasKey)
    {
        hasKey = false;

        if (IsIgnore(property)) return RedisValue.Null;

        if (HasKey(property))
        {
            hasKey = true;
            return RedisValue.Null;
        }

        TryGetField(property, out var field);

        return field;
    }

    public virtual string? GetKeyPrefix(Type type)
        => type.GetCustomAttribute<RedisKeyPrefixAttribute>()?.Prefix;

    public virtual object GetUtf8Formatter(PropertyInfo property)
        => Utf8FormatterVar.GetFormatter(property.PropertyType) ?? throw Ex.Utf8FormatterNotFound(property.PropertyType);

    public RedisValueWriter<TEntity>? GetWriter<TEntity>(PropertyInfo property)
        => Compiler.GetWriter<TEntity>(property);

    public RedisValueReader<TEntity>? GetReader<TEntity>(PropertyInfo property)
        => Compiler.GetReader<TEntity>(property);

    protected virtual bool HasKey(PropertyInfo property) => property.GetCustomAttribute<RedisKeyAttribute>() != null;

    protected virtual bool IsIgnore(PropertyInfo property) => property.GetCustomAttribute<RedisFieldIgnoreAttribute>() != null;

    protected virtual bool TryGetField(PropertyInfo property, out RedisValue field)
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