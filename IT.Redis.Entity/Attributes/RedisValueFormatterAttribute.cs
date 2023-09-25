namespace IT.Redis.Entity.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public abstract class RedisValueFormatterAttribute : Attribute
{
    internal abstract object GetFormatterObject();
}