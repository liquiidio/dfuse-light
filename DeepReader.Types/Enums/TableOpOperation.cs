using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TableOpOperation
{
    UNKNOWN = 0,
    INS = 1,
    REM = 2
}