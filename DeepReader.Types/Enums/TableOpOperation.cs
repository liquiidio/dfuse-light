using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TableOpOperation : byte
{
    UNKNOWN = 0,
    INS = 1,
    REM = 2
}