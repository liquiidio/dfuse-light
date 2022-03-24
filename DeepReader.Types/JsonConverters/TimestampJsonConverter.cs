using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.JsonConverters;

internal class TimestampJsonConverter : JsonConverter<Timestamp>
{
    public override Timestamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Timestamp(){ _ticks = (uint)DateTimeOffset.Parse(reader.GetString() ?? "").ToUnixTimeSeconds() };
    }

    public override void Write(Utf8JsonWriter writer, Timestamp value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToDateTime().ToString(CultureInfo.InvariantCulture));
    }

}