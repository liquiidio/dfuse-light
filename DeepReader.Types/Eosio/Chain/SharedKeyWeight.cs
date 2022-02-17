using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/authority.hpp
/// </summary>
public class SharedKeyWeight
{
    public PublicKey Key = PublicKey.Empty;   // for now public key, is SharedPublicKey in EOSIO (see below)
    public WeightType Weight;
}