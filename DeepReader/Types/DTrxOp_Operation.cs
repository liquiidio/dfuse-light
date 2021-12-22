namespace DeepReader.Types;

public enum DTrxOp_Operation
{
    DTrxOp_OPERATION_UNKNOWN       = 0,
    DTrxOp_OPERATION_CREATE        = 1,
    DTrxOp_OPERATION_PUSH_CREATE   = 2,
    DTrxOp_OPERATION_FAILED        = 3,
    DTrxOp_OPERATION_CANCEL        = 4,
    DTrxOp_OPERATION_MODIFY_CANCEL = 5,
    DTrxOp_OPERATION_MODIFY_CREATE = 6
}