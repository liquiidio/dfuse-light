﻿using DeepReader.EosTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepReader
{
    internal class NameJsonConverter : JsonConverter<Name>
    {
        public override Name? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Name(reader.GetString() ?? "");
        }

        public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
