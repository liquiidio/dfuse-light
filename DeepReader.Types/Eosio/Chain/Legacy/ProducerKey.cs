using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerKey
{
    public Name AccountName = Name.Empty;
    public PublicKey[] BlockSigningKey = Array.Empty<PublicKey>();//ecc.PublicKey
}