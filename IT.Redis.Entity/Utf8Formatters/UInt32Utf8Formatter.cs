using IT.Redis.Entity.Internal;
using System.Buffers.Text;

namespace IT.Redis.Entity.Utf8Formatters;

public static class UInt32Utf8Formatter
{
    public class Var : IUtf8Formatter<UInt32>
    {
        public static readonly Var Default = new();

        public int GetLength(in uint value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            return value <= 9 ? 1 : (int)Math.Floor(Math.Log10(value)) + 1;
        }

        public bool TryFormat(in uint value, Span<byte> span, out int written)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            return Utf8Formatter.TryFormat(value, span, out written);
        }
    }

    public class Fixed : IUtf8Formatter<UInt32>
    {
        public static readonly Fixed L1 = new(1);
        public static readonly Fixed L2 = new(2);
        public static readonly Fixed L3 = new(3);
        public static readonly Fixed L4 = new(4);
        public static readonly Fixed L5 = new(5);
        public static readonly Fixed L6 = new(6);
        public static readonly Fixed L7 = new(7);
        public static readonly Fixed L8 = new(8);
        public static readonly Fixed L9 = new(9);
        public static readonly Fixed L10 = new(10);

        private readonly byte _length;

        public Fixed(byte length)
        {
            if (length < 1 || length > 10) throw new ArgumentOutOfRangeException(nameof(length));

            _length = length;
        }

        public int GetLength(in uint value)
        {
            if (value < 0 || value > MaxValue.UInt32(_length)) throw new ArgumentOutOfRangeException(nameof(value));

            return _length;
        }

        public bool TryFormat(in uint value, Span<byte> span, out int written)
        {
            if (value < 0 || value > MaxValue.UInt32(_length)) throw new ArgumentOutOfRangeException(nameof(value));

            return Utf8Formatter.TryFormat(value, span, out written, new System.Buffers.StandardFormat('d', _length));
        }
    }
}