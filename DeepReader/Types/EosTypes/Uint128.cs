﻿namespace DeepReader.Types.EosTypes;

public class Uint128
{
    private readonly byte[] _value;

    public Uint128()
    {
        _value = Array.Empty<byte>();
    }

    public Uint128(byte[] value)
    {
        if (value.Length != 16)
            throw new ArgumentException("Uint128 must be 16 bytes long");
        _value = value;
    }

    public static implicit operator Uint128(byte[] value)
    {
        return new Uint128(value);
    }

    public static implicit operator byte[](Uint128 value)
    {
        return value._value;
    }
}