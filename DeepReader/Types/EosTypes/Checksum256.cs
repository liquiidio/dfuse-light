using DeepReader.JsonConverters;
using System.Text.Json.Serialization;

namespace DeepReader.EosTypes
{
    [JsonConverter(typeof(Checksum256JsonConverter))]
    public class Checksum256
    {
        private string _value = string.Empty;

        public static implicit operator Checksum256(string value)
        {
            return new(value);
        }

        public static implicit operator string(Checksum256 value)
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

        public Checksum256(string value)
        {
            _value = value;
        }

        public static Checksum256 Empty => new("");
    }
}
