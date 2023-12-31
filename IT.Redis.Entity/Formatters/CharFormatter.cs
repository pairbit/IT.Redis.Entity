﻿namespace IT.Redis.Entity.Formatters;

public class CharFormatter : NullableFormatter<Char>
{
    public static readonly CharFormatter Default = new();

    public override void DeserializeNotNull(in RedisValue redisValue, ref Char value) => value = checked((char)(uint)redisValue);

    public override RedisValue Serialize(in Char value) => (uint)value;
}