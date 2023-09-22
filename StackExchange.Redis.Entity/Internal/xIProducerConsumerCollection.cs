using System.Collections.Concurrent;

namespace StackExchange.Redis.Entity.Internal;

internal static class xIProducerConsumerCollection
{
    public static void AddOrThrow<T>(this IProducerConsumerCollection<T> collection, T value)
    {
        if (!collection.TryAdd(value)) throw Ex.FailedAddCollection(collection.GetType());
    }

    public static void ClearOrThrow<T>(this IProducerConsumerCollection<T> collection)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
        if (collection is ConcurrentBag<T> bag)
        {
            bag.Clear();
            return;
        }

        if (collection is ConcurrentQueue<T> queue)
        {
            if (!queue.IsEmpty) queue.Clear();
            return;
        }
#endif
        if (collection is ConcurrentStack<T> stack)
        {
            stack.Clear();
            return;
        }

        throw Ex.ClearNotSupported(collection.GetType());
    }
}