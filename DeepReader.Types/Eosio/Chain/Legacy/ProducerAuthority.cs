using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerAuthority
{
    public Name AccountName = Name.Empty;
    public BlockSigningAuthorityVariant BlockSigningAuthority = new BlockSigningAuthorityV0();
}