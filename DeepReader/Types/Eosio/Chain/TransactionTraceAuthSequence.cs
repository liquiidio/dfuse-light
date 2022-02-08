using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// flat_map<account_name,uint64_t>
/// </summary>
public class TransactionTraceAuthSequence {
    public Name Account = string.Empty;
    public ulong Sequence = 0;
}