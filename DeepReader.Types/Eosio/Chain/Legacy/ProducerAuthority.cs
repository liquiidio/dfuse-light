using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerAuthority : IEosioSerializable<ProducerAuthority>
{
    public Name AccountName;
    public BlockSigningAuthorityVariant BlockSigningAuthority;

    public ProducerAuthority(BinaryReader reader)
    {
        AccountName = Name.ReadFromBinaryReader(reader);
        BlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);
    }
    public static ProducerAuthority ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new ProducerAuthority(reader);
    }
}