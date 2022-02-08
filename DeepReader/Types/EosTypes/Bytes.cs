using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepReader.EosTypes
{
    public class Bytes
    {
        internal byte[] _value = Array.Empty<byte>();

        public Bytes()
        {

        }

        public static implicit operator Bytes(byte[] value)
        {
            return new() { _value = value };
        }

        public static implicit operator byte[](Bytes value)
        {
            return value._value;
        }

        public string ToJson()
        {
            return SerializationHelper.ByteArrayToHexString(_value);
        }
    }

    public class Bytes<T> : Bytes
    {
        public T? Instance;

        [JsonIgnore]
        public bool IsDeserialized => Instance != null;

        public Bytes()
        {
            Instance = default(T);
        }

        public Bytes(T instance)
        {
            Instance = instance;
        }

        public T? GetInstance()
        {
            return Instance;
        }

        public async Task DeserializeAsync(CancellationToken cancellationToken)
        {
            if (IsDeserialized)
                return;

            Instance = await Deserializer.Deserializer.DeserializeAsync<T>(_value, cancellationToken);
        }

        public void Deserialize()
        {
            if (IsDeserialized)
                return;

            Instance = Deserializer.Deserializer.Deserialize<T>(_value);
        }

        public static implicit operator Bytes<T>(byte[] value)
        {
            return new() { _value = value };
        }

        public static implicit operator byte[](Bytes<T> value)
        {
            return value._value;
        }
    }
}
