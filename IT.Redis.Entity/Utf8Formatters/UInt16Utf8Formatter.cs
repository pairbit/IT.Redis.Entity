using IT.Redis.Entity.Internal;
using System.Buffers.Text;

namespace IT.Redis.Entity.Utf8Formatters;

public static class UInt16Utf8Formatter
{
    public class Var : IUtf8Formatter<UInt16>
    {
        public static readonly Var Default = new();

        public int GetLength(in ushort value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            return value <= 9 ? 1 : (int)Math.Floor(Math.Log10(value)) + 1;
        }

        public bool TryFormat(in ushort value, Span<byte> span, out int written)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            return Utf8Formatter.TryFormat(value, span, out written);
        }
    }

    public class Fixed : IUtf8Formatter<UInt16>
    {
        public static readonly Fixed L1 = new(1);
        public static readonly Fixed L2 = new(2);
        public static readonly Fixed L3 = new(3);
        public static readonly Fixed L4 = new(4);
        public static readonly Fixed L5 = new(5);

        private readonly byte _length;

        public Fixed(byte length)
        {
            if (length < 1 || length > 5) throw new ArgumentOutOfRangeException(nameof(length));

            _length = length;
        }

        public int GetLength(in ushort value)
        {
            if (value < 0 || value > MaxValue.UInt16(_length)) throw new ArgumentOutOfRangeException(nameof(value));

            return _length;
        }

        public bool TryFormat(in ushort value, Span<byte> span, out int written)
        {
            if (value < 0 || value > MaxValue.UInt16(_length)) throw new ArgumentOutOfRangeException(nameof(value));

            return Utf8Formatter.TryFormat(value, span, out written, new System.Buffers.StandardFormat('d', _length));
        }
    }
}