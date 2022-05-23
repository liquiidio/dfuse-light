using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Types.JsonConverters;

public class FlattenedDbOpJsonConverter : JsonConverter<FlattenedDbOp>
{
    public override FlattenedDbOp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, FlattenedDbOp value, JsonSerializerOptions options)
    {
        // TODO, just a temporary workaround
        writer.WriteStartObject();
        writer.WriteString("operation", value.Operation.ToString());
        writer.WriteString("code", value.Code);
        writer.WriteString("scope", value.Scope);
        writer.WriteString("tableName", value.TableName);
        writer.WriteString("primaryKey", value.PrimaryKey);
        writer.WriteString("oldPayer", value.OldPayer);
        writer.WriteString("newPayer", value.NewPayer);
        writer.WriteEndObject();
    }

}