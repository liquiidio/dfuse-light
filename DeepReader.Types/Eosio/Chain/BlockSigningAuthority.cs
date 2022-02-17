namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// /// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public abstract class BlockSigningAuthorityVariant
{
}

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class BlockSigningAuthorityV0 : BlockSigningAuthorityVariant
{
    public uint Threshold = 0;
    public SharedKeyWeight[] Keys = Array.Empty<SharedKeyWeight>();
}