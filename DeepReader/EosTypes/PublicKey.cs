using DeepReader.JsonConverters;
using System.Text.Json.Serialization;

namespace DeepReader.EosTypes
{
    [JsonConverter(typeof(PublicKeyJsonConverter))]
    public class PublicKey
    {
        private string _value;

        public static implicit operator PublicKey(string value)
        {
            return new(value);
        }

        public static implicit operator string(PublicKey value)
        {
            return value._value;
        }

        public string ToJson()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value;
        }

        public PublicKey(string value)
        {
            _value = value;
        }
    }
}