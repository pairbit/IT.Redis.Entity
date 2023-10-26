namespace IT.Redis.Entity.Internal;

internal static class MaxValue
{
    public static sbyte SByte(byte length)
    {
        if (length == 1) return 9;
        if (length == 2) return 99;

        return sbyte.MaxValue;
    }

    public static byte Byte(byte length)
    {
        if (length == 1) return 9;
        if (length == 2) return 99;

        return byte.MaxValue;
    }

    public static short Int16(byte length)
    {
        if (length == 1) return 9;
        if (length == 2) return 99;
        if (length == 3) return 999;
        if (length == 4) return 9999;

        return short.MaxValue;
    }

    public static ushort UInt16(byte length)
    {
        if (length == 1) return 9;
        if (length == 2) return 99;
        if (length == 3) return 999;
        if (length == 4) return 9999;

        return ushort.MaxValue;
    }

    public static int Int32(byte length)
    {
        if (length == 1) return 9;
        if (length == 2) return 99;
        if (length == 3) return 999;
        if (length == 4) return 9999;
        if (length == 5) return 99999;
        if (length == 6) return 999999;
        if (length == 7) return 9999999;
        if (length == 8) return 99999999;
        if (length == 9) return 999999999;

        return int.MaxValue;
    }

    public static uint UInt32(byte length)
    {
        if (length == 1) return 9;
        if (length == 2) return 99;
        if (length == 3) return 999;
        if (length == 4) return 9999;
        if (length == 5) return 99999;
        if (length == 6) return 999999;
        if (length == 7) return 9999999;
        if (length == 8) return 99999999;
        if (length == 9) return 999999999;

        return uint.MaxValue;
    }
}