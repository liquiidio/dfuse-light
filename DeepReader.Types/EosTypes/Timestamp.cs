﻿using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(TimestampJsonConverter))]
public sealed class Timestamp : BinaryType
{
    public uint Ticks;

    public static implicit operator Timestamp(uint value)
    {
        return new Timestamp() {Ticks = value};
    }

    public DateTime ToDateTime()
    {
        return DateTimeOffset.FromUnixTimeSeconds(Ticks).DateTime;
    }

    public static Timestamp Zero => new();
}