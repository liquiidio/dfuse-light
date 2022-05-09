using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RlimitOpOperation
{
    UNKNOWN = 0,
    INS = 1,
    UPD = 2
}