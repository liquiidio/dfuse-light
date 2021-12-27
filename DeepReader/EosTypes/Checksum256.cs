using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.EosTypes
{
    public class Checksum256
    {
        private string _value = string.Empty;

        public static implicit operator Checksum256(string value)
        {
            return new() { _value = value };
        }

        public static implicit operator string(Checksum256 value)
        {
            return value._value;
        }
    }
}
