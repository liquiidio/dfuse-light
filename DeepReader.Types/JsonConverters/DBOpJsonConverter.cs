using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

public class DbOpJsonConverter : JsonConverter<DbOp>
{
    public override DbOp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, DbOp value, JsonSerializerOptions options)
    {
        // TODO, just a temporary workaround
        writer.WriteRawValue(JsonSerializer.SerializeToUtf8Bytes(value));
    }

}