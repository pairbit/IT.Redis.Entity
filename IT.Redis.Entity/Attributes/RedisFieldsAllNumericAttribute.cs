namespace IT.Redis.Entity.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]

public class RedisFieldsAllNumericAttribute : Attribute
{
}