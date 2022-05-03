using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;
using DeepReader.Types.Other;

namespace DeepReader.Types.JsonConverters;

internal class NameJsonConverter : JsonConverter<Name>
{
    public override Name Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var nameString = reader.GetString();
        if (nameString != null)
        {
            var binary = SerializationHelper.NameToBytes(nameString);
            return NameCache.GetOrCreate(binary);
        }

        if (reader.TryGetUInt64(out var nameLong))
        {
            return NameCache.GetOrCreate(nameLong);
        }

        return Name.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.StringVal);
    }
}