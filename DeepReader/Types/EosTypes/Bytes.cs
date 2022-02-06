using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
