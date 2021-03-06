using System.Text.Json.Serialization;

namespace DeepReader.Types.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RamOpOperation : byte
{
    UNKNOWN = 0,
    CREATE_TABLE = 1,
    DEFERRED_TRX_ADD = 2,
    DEFERRED_TRX_CANCEL = 3,
    DEFERRED_TRX_PUSHED = 4,
    DEFERRED_TRX_RAM_CORRECTION = 5,
    DEFERRED_TRX_REMOVED = 6,
    DELETEAUTH = 7,
    LINKAUTH = 8,
    NEWACCOUNT = 9,
    PRIMARY_INDEX_ADD = 10,
    PRIMARY_INDEX_REMOVE = 11,
    PRIMARY_INDEX_UPDATE = 12,
    PRIMARY_INDEX_UPDATE_ADD_NEW_PAYER = 13,
    PRIMARY_INDEX_UPDATE_REMOVE_OLD_PAYER = 14,
    REMOVE_TABLE = 15,
    SECONDARY_INDEX_ADD = 16,
    SECONDARY_INDEX_REMOVE = 17,
    SECONDARY_INDEX_UPDATE_ADD_NEW_PAYER = 18,
    SECONDARY_INDEX_UPDATE_REMOVE_OLD_PAYER = 19,
    SETABI = 20,
    SETCODE = 21,
    UNLINKAUTH = 22,
    UPDATEAUTH_CREATE = 23,
    UPDATEAUTH_UPDATE = 24
}