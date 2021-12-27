namespace DeepReader.EosTypes
{
    public class VarInt64
    {
        private long _value;

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