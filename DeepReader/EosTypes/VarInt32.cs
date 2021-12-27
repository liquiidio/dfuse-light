namespace DeepReader.EosTypes
{
    public class VarInt32
    {
        private int _value;

        public static implicit operator VarInt32(int value)
        {
            return new() { _value = value };
        }

        public static implicit operator int(VarInt32 value)
        {
            return value._value;
        }
    }
}