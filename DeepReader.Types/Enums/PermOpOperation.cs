using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PermOpOperation
{
    UNKNOWN = 0,
    INS = 1,
    UPD = 2,
    REM = 3
}