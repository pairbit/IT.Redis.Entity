namespace IT.Redis.Entity.Tests;

internal static class Shared
{
    static readonly string ConnectionString =
        "localhost:6379,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False";

    public static readonly ConnectionMultiplexer Connection;
    public static readonly IDatabase Db;

    static Shared()
    {
        Connection = ConnectionMultiplexer.Connect(ConnectionString);
        Db = Connection.GetDatabase();
    }
}