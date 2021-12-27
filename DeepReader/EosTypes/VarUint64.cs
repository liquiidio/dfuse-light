namespace DeepReader.EosTypes
{
    public class VarUint64
    {
        private ulong _value;

        public static implicit operator VarUint64(ulong value)
        {
            return new() { _value = value };
        }

        public static implicit operator ulong(VarUint64 value)
        {
            return value._value;
        }
    }
}