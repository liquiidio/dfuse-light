using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrxOpOperation
{
    UNKNOWN = 0,
    CREATE = 1
}