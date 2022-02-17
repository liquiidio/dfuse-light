using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class SignedTransaction : Transaction
{
    [SortOrder(10)]
    public Signature[] Signatures = Array.Empty<Signature>();

    [SortOrder(11)]
    public Bytes[] ContextFreeData = Array.Empty<Bytes>(); //< for each context-free action, there is an entry here
}