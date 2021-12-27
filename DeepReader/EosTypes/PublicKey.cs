namespace DeepReader.EosTypes
{
    public class PublicKey
    {
        private string _value;

        public static implicit operator PublicKey(string value)
        {
            return new() { _value = value };
        }

        public static implicit operator string(PublicKey value)
        {
            return value._value;
        }

        public string ToJson()
        {
            return _value;
        }
    }
}