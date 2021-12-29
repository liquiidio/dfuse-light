using System.Text.Json.Serialization;

namespace DeepReader.EosTypes
{
    [JsonConverter(typeof(NameJsonConverter))]
    public class Name
    {
        private readonly ulong _binary;

        private readonly string _value;


        public static implicit operator string(Name value)
        {
            return value._value;
        }

        public static implicit operator ulong(Name value)
        {
            return value._binary;
        }

        public static implicit operator Name(string value)
        {
            return new Name(0, value);  // TODO string to ulong
        }
        public string ToJson()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value;
        }

        public Name(string value)
        {
            _value = value;
            _binary = 0;// todo string to ulong
        }

        public Name(ulong binary, string value)
        {
            _value = value;
            _binary = binary;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Name item)
            {
                return false;
            }

            return _binary.Equals(item._binary) || _value.Equals(item._value);
        }

        public static bool operator ==(Name obj1, Name obj2)
        {
            return obj1?._binary == obj2?._binary;
        }

        public static bool operator !=(Name obj1, Name obj2)
        {
            return obj1?._binary != obj2?._binary;
        }

        public override int GetHashCode()
        {
            return _binary.GetHashCode();
        }


    }
}