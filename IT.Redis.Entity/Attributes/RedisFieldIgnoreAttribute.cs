namespace IT.Redis.Entity.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class RedisFieldIgnoreAttribute : Attribute
{

}