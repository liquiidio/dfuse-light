namespace DeepReader.Types;

public enum TransactionStatus
{
    TransactionStatus_TRANSACTIONSTATUS_NONE     = 0,
    TransactionStatus_TRANSACTIONSTATUS_EXECUTED = 1,
    TransactionStatus_TRANSACTIONSTATUS_SOFTFAIL = 2,
    TransactionStatus_TRANSACTIONSTATUS_HARDFAIL = 3,
    TransactionStatus_TRANSACTIONSTATUS_DELAYED  = 4,
    TransactionStatus_TRANSACTIONSTATUS_EXPIRED  = 5,
    TransactionStatus_TRANSACTIONSTATUS_UNKNOWN  = 6,
    TransactionStatus_TRANSACTIONSTATUS_CANCELED = 7
}