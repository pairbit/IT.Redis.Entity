﻿using System.Text;

namespace IT.Redis.Entity.Internal;

internal readonly struct BuildState
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