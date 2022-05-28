﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepReader.Types.JsonConverters;

public sealed class DbOpJsonConverter : JsonConverter<DbOp>
{
    public override DbOp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, DbOp value, JsonSerializerOptions options)
    {
        // TODO, just a temporary workaround
        writer.WriteStartObject();
        writer.WriteString("operation", value.Operation.ToString());
        writer.WriteString("code", value.Code);
        writer.WriteString("scope", value.Scope);
        writer.WriteString("primaryKey", value.PrimaryKey.Span);
        writer.WriteString("oldPayer", value.OldPayer);
        writer.WriteString("newPayer", value.NewPayer);
        writer.WriteEndObject();
    }
}