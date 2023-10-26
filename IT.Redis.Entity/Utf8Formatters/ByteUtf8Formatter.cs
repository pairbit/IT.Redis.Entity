using IT.Redis.Entity.Internal;
using System.Buffers.Text;

namespace IT.Redis.Entity.Utf8Formatters;

public static class ByteUtf8Formatter
{
    public class Var : IUtf8Formatter<byte>
    {
        public static readonly Var Default = new();

        public int GetLength(in byte value)
        {
            return value <= 9 ? 1 : (int)Math.Floor(Math.Log10(value)) + 1;
        }

        public bool TryFormat(in byte value, Span<byte> span, out int written)
        {
            return Utf8Formatter.TryFormat(value, span, out written);
        }
    }

    public class Fixed : IUtf8Formatter<byte>
    {
        public static readonly Fixed L1 = new(1);
        public static readonly Fixed L2 = new(2);
        public static readonly Fixed L3 = new(3);

        private readonly byte _length;

        public Fixed(byte length)
        {
            if (length < 1 || length > 3) throw new ArgumentOutOfRangeException(nameof(length));

            _length = length;
        }

        public int GetLength(in byte value)
        {
            if (value > MaxValue.Byte(_length)) throw new ArgumentOutOfRangeException(nameof(value));

            return _length;
        }

        public bool TryFormat(in byte value, Span<byte> span, out int written)
        {
            if (value > MaxValue.Byte(_length)) throw new ArgumentOutOfRangeException(nameof(value));

            return Utf8Formatter.TryFormat(value, span, out written, new System.Buffers.StandardFormat('d', _length));
        }
    }
}