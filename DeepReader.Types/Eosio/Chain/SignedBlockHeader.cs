using DeepReader.Types.Helpers;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class SignedBlockHeader : BlockHeader
{
    [SortOrder(10)]
    public Signature ProducerSignature = Signature.Empty;// ecc.Signature // no pointer!!
}