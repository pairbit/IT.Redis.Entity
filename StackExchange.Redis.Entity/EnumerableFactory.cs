namespace StackExchange.Redis.Entity;

public class EnumerableFactory : IEnumerableFactory
{
    public virtual TEnumerable New<TEnumerable, T>(int capacity) where TEnumerable : IEnumerable<T>
    {
        var type = typeof(TEnumerable);
        if (type.IsSZArray)
        {
            return (TEnumerable)(object)(capacity == 0 ? Array.Empty<T>() : new T[capacity]);
        }
        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(List<>))
            {
                return (TEnumerable)(object)new List<T>(capacity);
            }
        }

        throw new NotSupportedException($"Creating an instance for type {type.FullName} is not supported");
    }

    class Cache<T>
    {
        static Cache()
        {

        }
    }
}