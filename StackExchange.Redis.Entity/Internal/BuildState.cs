using System.Text;

namespace StackExchange.Redis.Entity.Internal;

public readonly struct BuildState
{
    public readonly ReadOnlyMemory<byte> Memory;
    public readonly Encoding Encoding;
    public readonly int Length;

    public BuildState(ReadOnlyMemory<byte> memory, Encoding encoding, int length)
    {
        Memory = memory;
        Encoding = encoding;
        Length = length;
    }
}