﻿namespace DeepReader.EosTypes
{
    public class Int128
    {
        private byte[] _value;

        public static implicit operator Int128(byte[] value)
        {
            return new() { _value = value };
        }

        public static implicit operator byte[](Int128 value)
        {
            return value._value;
        }
    }
}