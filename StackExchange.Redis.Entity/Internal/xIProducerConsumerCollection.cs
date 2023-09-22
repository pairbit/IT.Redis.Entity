using System.Collections.Concurrent;

namespace StackExchange.Redis.Entity.Internal;

internal static class xIProducerConsumerCollection
{
    public static void AddOrThrow<T>(this IProducerConsumerCollection<T> collection, T value)
    {
        if (!collection.TryAdd(value)) throw Ex.FailedAddCollection(collection.GetType());
    }
}