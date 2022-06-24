using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerAuthority : IEosioSerializable<ProducerAuthority>
{
    public Name AccountName;
    public BlockSigningAuthorityVariant BlockSigningAuthority;

    public ProducerAuthority(BinaryBufferReader reader)
    {
        AccountName = Name.ReadFromBinaryReader(reader);
        BlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);
    }
    public static ProducerAuthority ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new ProducerAuthority(reader);
    }
}