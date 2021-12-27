using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class TransactionReceipt
{
    public string Id = string.Empty;//string
    public ulong Index = 0;//uint64
    public TransactionStatus Status = TransactionStatus.UNKNOWN;//TransactionStatus
    public uint CpuUsageMicroSeconds = 0;//uint32
    public uint NetUsageWords = 0;//uint32
    public PackedTransaction PackedTransaction = new PackedTransaction();//*PackedTransaction
}