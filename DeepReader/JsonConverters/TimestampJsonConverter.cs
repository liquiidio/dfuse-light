﻿using DeepReader.EosTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.JsonConverters
{
    internal class TimestampJsonConverter : JsonConverter<Timestamp>
    {
        public override Timestamp? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Timestamp((uint)DateTimeOffset.Parse(reader.GetString() ?? "").ToUnixTimeSeconds());
        }

        public override void Write(Utf8JsonWriter writer, Timestamp value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToDateTime().ToString());
        }

    }
}
