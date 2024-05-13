#if NETCOREAPP3_1_OR_GREATER

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace IT.Redis.Entity.Configurations;

public class DataAnnotationConfiguration : AnnotationConfiguration
{
    public DataAnnotationConfiguration() { }

    public DataAnnotationConfiguration(IRedisValueFormatter formatter) : base(formatter) { }

    public override string? GetKeyPrefix(Type type)
    {
        var table = type.GetCustomAttribute<TableAttribute>();
        if (table != null) return table.Schema == null ? table.Name : $"{table.Schema}:{table.Name}";

        return base.GetKeyPrefix(type);
    }

    public override bool IsKey(PropertyInfo property)
        => property.GetCustomAttribute<KeyAttribute>() != null || base.IsKey(property);

    public override bool IsIgnore(PropertyInfo property)
        => property.GetCustomAttribute<NotMappedAttribute>() != null || base.IsIgnore(property);

    public override bool TryGetField(PropertyInfo property, out RedisValue field)
    {
        var column = property.GetCustomAttribute<ColumnAttribute>();
        if (column != null)
        {
            if (column.Order >= 0)
            {
                field = column.Order;
                return true;
            }

            if (column.Name != null)
            {
                field = column.Name;
                return true;
            }
        }

        return base.TryGetField(property, out field);
    }
}

#endif