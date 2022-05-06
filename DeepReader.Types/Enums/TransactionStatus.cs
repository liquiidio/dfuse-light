using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionStatus
{
    NONE = 0,
    EXECUTED = 1,
    SOFTFAIL = 2,
    HARDFAIL = 3,
    DELAYED = 4,
    EXPIRED = 5,
    UNKNOWN = 6,
    CANCELED = 7
}