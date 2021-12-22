namespace DeepReader.Types;

public enum RAMOp_Operation
{
    RAMOp_OPERATION_UNKNOWN                                 = 0,
    RAMOp_OPERATION_CREATE_TABLE                            = 1,
    RAMOp_OPERATION_DEFERRED_TRX_ADD                        = 2,
    RAMOp_OPERATION_DEFERRED_TRX_CANCEL                     = 3,
    RAMOp_OPERATION_DEFERRED_TRX_PUSHED                     = 4,
    RAMOp_OPERATION_DEFERRED_TRX_RAM_CORRECTION             = 5,
    RAMOp_OPERATION_DEFERRED_TRX_REMOVED                    = 6,
    RAMOp_OPERATION_DELETEAUTH                              = 7,
    RAMOp_OPERATION_LINKAUTH                                = 8,
    RAMOp_OPERATION_NEWACCOUNT                              = 9,
    RAMOp_OPERATION_PRIMARY_INDEX_ADD                       = 10,
    RAMOp_OPERATION_PRIMARY_INDEX_REMOVE                    = 11,
    RAMOp_OPERATION_PRIMARY_INDEX_UPDATE                    = 12,
    RAMOp_OPERATION_PRIMARY_INDEX_UPDATE_ADD_NEW_PAYER      = 13,
    RAMOp_OPERATION_PRIMARY_INDEX_UPDATE_REMOVE_OLD_PAYER   = 14,
    RAMOp_OPERATION_REMOVE_TABLE                            = 15,
    RAMOp_OPERATION_SECONDARY_INDEX_ADD                     = 16,
    RAMOp_OPERATION_SECONDARY_INDEX_REMOVE                  = 17,
    RAMOp_OPERATION_SECONDARY_INDEX_UPDATE_ADD_NEW_PAYER    = 18,
    RAMOp_OPERATION_SECONDARY_INDEX_UPDATE_REMOVE_OLD_PAYER = 19,
    RAMOp_OPERATION_SETABI                                  = 20,
    RAMOp_OPERATION_SETCODE                                 = 21,
    RAMOp_OPERATION_UNLINKAUTH                              = 22,
    RAMOp_OPERATION_UPDATEAUTH_CREATE                       = 23,
    RAMOp_OPERATION_UPDATEAUTH_UPDATE                       = 24
}