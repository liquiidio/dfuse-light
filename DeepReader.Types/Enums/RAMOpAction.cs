using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RamOpAction : byte
{
    UNKNOWN = 0,
    ADD = 1,
    CANCEL = 2,
    CORRECTION = 3,
    PUSH = 4,
    REMOVE = 5,
    UPDATE = 6
}