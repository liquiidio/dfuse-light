using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class SignedBlock : SignedBlockHeader
{
    [SortOrder(11)]
    public TransactionReceipt[] Transactions = Array.Empty<TransactionReceipt>();
    [SortOrder(12)]
    public Extension[] BlockExtensions = Array.Empty<Extension>();
}