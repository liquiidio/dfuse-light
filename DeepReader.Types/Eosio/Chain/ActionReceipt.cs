using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/action_receipt.hpp
/// </summary>
public class ActionReceipt {
    public Name Receiver = string.Empty;
    public Checksum256 ActionDigest = string.Empty;
    public ulong GlobalSequence = 0;
    public ulong ReceiveSequence = 0;
    public TransactionTraceAuthSequence[] AuthSequence = Array.Empty<TransactionTraceAuthSequence>();
    public VarUint32 CodeSequence = 0;
    public VarUint32 AbiSequence = 0;
}