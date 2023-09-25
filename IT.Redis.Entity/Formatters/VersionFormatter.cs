using IT.Redis.Entity.Internal;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IT.Redis.Entity.Formatters;

public class VersionFormatter : IRedisValueFormatter<Version>
{
    public static readonly VersionFormatter Default = new();

    public void Deserialize(in RedisValue redisValue, ref Version? value)
    {
        if (redisValue == RedisValue.EmptyString)
        {
            value = null;
        }
        else
        {
            var span = ((ReadOnlyMemory<byte>)redisValue).Span;

            if (span.Length != 16) throw Ex.InvalidLength(typeof(Version), 16);

            ref byte spanRef = ref MemoryMarshal.GetReference(span);

            int major = Unsafe.ReadUnaligned<int>(ref spanRef);
            int minor = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, 4));
            int build = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, 8));
            int revision = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref spanRef, 12));

            value = revision == -1 ? build == -1 ? new Version(major, minor) : new Version(major, minor, build) : new Version(major, minor, build, revision);
        }
    }

    public RedisValue Serialize(in Version? value)
    {
        if (value == null) return RedisValue.EmptyString;

        var bytes = new byte[16];

        Unsafe.WriteUnaligned(ref bytes[0], value.Major);
        Unsafe.WriteUnaligned(ref bytes[4], value.Minor);
        Unsafe.WriteUnaligned(ref bytes[8], value.Build);
        Unsafe.WriteUnaligned(ref bytes[12], value.Revision);

        return bytes;
    }
}