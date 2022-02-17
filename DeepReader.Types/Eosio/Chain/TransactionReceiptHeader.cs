using DeepReader.Types.Helpers;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class TransactionReceiptHeader
{
    [SortOrder(1)]
    public byte Status;    // fc::enum_type<uint8_t,status_enum> v
    [SortOrder(2)]
    public uint CpuUsageUs;
    [SortOrder(3)]
    public VarUint32 NetUsageWords = 0;
}