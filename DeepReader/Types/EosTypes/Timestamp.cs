using DeepReader.JsonConverters;
using System.Text.Json.Serialization;

namespace DeepReader.EosTypes
{
    [JsonConverter(typeof(TimestampJsonConverter))]
    public class Timestamp
    {
        private readonly uint _binary;

        public static implicit operator Timestamp(uint value)
        {
            return new (value);
        }

        public DateTime ToDateTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(_binary).ToLocalTime().DateTime;
        }

        public Timestamp(uint binary) 
        { 
            _binary = binary; 
        }
    }
}
