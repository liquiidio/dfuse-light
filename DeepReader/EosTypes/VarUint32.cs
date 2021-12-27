namespace DeepReader.EosTypes
{
    public class VarUint32
    {
        private uint _value;

        public static implicit operator VarUint32(uint value)
        {
            return new() { _value = value };
        }

        public static implicit operator uint(VarUint32 value)
        {
            return value._value;
        }

        public VarUint32()
        {

        }

        public VarUint32(uint value)
        {
            _value = value;
        }

        public uint ToUint()
        {
            return _value;
        }
    }
}
