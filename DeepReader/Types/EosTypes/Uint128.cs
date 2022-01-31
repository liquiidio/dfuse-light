namespace DeepReader.EosTypes
{
    public class Uint128
    {
        private byte[] _value;

        public static implicit operator Uint128(byte[] value)
        {
            return new() { _value = value };
        }

        public static implicit operator byte[](Uint128 value)
        {
            return value._value;
        }
    }
}