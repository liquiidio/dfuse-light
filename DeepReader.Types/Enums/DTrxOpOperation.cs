namespace DeepReader.Types.Enums;

public enum DTrxOpOperation
{
    UNKNOWN = 0,
    CREATE = 1,
    PUSH_CREATE = 2,
    FAILED = 3,
    CANCEL = 4,
    MODIFY_CANCEL = 5,
    MODIFY_CREATE = 6
}