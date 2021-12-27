namespace DeepReader.EosTypes
{
    public class Checksum160
    {
        private string _value;

        public static implicit operator Checksum160(string value)
        {
            return new() { _value = value };
        }

        public static implicit operator string(Checksum160 value)
        {
            return value._value;
        }
    }
}