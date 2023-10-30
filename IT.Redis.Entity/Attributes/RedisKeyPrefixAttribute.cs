namespace IT.Redis.Entity.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public class RedisKeyPrefixAttribute : Attribute
{
    public string Prefix { get; set; }

    public RedisKeyPrefixAttribute(string prefix)
    {
        Prefix = prefix;
    }
}