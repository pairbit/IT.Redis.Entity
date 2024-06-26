﻿using System.Reflection;
using System.Runtime.Serialization;

namespace IT.Redis.Entity.Configurations;

public class DataContractAnnotationConfiguration : AnnotationConfiguration
{
    public DataContractAnnotationConfiguration() { }

    public DataContractAnnotationConfiguration(IRedisValueFormatter formatter) : base(formatter) { }

    //DataContractAttribute

    public override bool IsIgnore(PropertyInfo property)
        => property.GetCustomAttribute<IgnoreDataMemberAttribute>() != null || base.IsIgnore(property);

    public override bool TryGetField(PropertyInfo property, out RedisValue field)
    {
        var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
        if (dataMember != null)
        {
            if (dataMember.Order >= 0)
            {
                field = dataMember.Order;
                return true;
            }
            if (dataMember.IsNameSetExplicitly)
            {
                field = dataMember.Name;
                return true;
            }
        }
        return base.TryGetField(property, out field);
    }
}