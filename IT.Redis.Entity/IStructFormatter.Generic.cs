namespace IT.Redis.Entity;

public interface IStructFormatter<T> : IRedisValueFormatter<T?>, IRedisValueFormatter<T> where T : struct
{

}