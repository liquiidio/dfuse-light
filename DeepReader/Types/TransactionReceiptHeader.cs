using DeepReader.EosTypes;
using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class TransactionReceiptHeader
{
    public TransactionStatus Status = TransactionStatus.UNKNOWN;//TransactionStatus
    public uint CpuUsageMicroSeconds = 0;//uint32
    public VarUint32 NetUsageWords = 0;//uint32
}