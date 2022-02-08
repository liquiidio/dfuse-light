namespace DeepReader.Types.EosTypes
{
    public class Float128
    {
        private byte[] _value = Array.Empty<byte>();

        public static implicit operator Float128(byte[] value)
        {
            return new() { _value = value };
        }

        public static implicit operator byte[](Float128 value)
        {
            return value._value;
        }
    }
}