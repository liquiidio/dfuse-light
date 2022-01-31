namespace DeepReader.EosTypes
{
    public class Checksum512
    {
        private string _value;

        public static implicit operator Checksum512(string value)
        {
            return new() { _value = value };
        }

        public static implicit operator string(Checksum512 value)
        {
            return value._value;
        }
    }
}