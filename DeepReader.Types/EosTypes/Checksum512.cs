﻿using DeepReader.Types.Fc;

namespace DeepReader.Types.EosTypes;

public class Checksum512 : BinaryType
{
    private string _value = string.Empty;

    public static implicit operator Checksum512(string value)
    {
        return new Checksum512 { _value = value };
    }

    public static implicit operator string(Checksum512 value)
    {
        return value._value;
    }

    public static Checksum512 Empty => new();
}