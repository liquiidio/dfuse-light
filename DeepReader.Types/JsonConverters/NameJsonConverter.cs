using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Helpers;

namespace DeepReader.Types.JsonConverters;

internal class NameJsonConverter : JsonConverter<Name>
{
    public override Name Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var nameString = reader.GetString();
        if (nameString != null)
        {
            var binary = SerializationHelper.NameToBytes(nameString);
            return new Name() { StringVal = nameString, Binary = binary, IntVal = BitConverter.ToUInt64(binary) };
        }
        else if (reader.TryGetUInt64(out var nameLong))
        {
            var binary = BitConverter.GetBytes(nameLong);
            return new Name()
            { Binary = binary, StringVal = SerializationHelper.ByteArrayToName(binary), IntVal = nameLong };
        }
        else
            return Name.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}