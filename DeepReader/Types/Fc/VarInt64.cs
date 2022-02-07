namespace DeepReader.Types.Fc
{
    /// <summary>
    /// libraries/fc/include/fc/io/varint.hpp
    /// </summary>
    public class VarInt64
    {
        private long _value;

        public VarInt64()
        {

        }

        public VarInt64(long value)
        {
            _value = value;
        }

        public static implicit operator VarInt64(long value)
        {
            return new() { _value = value };
        }

        public static implicit operator long(VarInt64 value)
        {
            return value._value;
        }
    }
}