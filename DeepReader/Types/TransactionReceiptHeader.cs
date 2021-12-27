using DeepReader.Types.Enums;

namespace DeepReader.Types;

public class TransactionReceiptHeader
{
    public TransactionStatus Status { get; set; } = TransactionStatus.UNKNOWN;//TransactionStatus
    public uint CpuUsageMicroSeconds = 0;//uint32
    public uint NetUsageWords = 0;//uint32
}