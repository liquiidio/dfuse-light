using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerAuthority : IEosioSerializable<ProducerAuthority>
{
    public Name AccountName;
    public BlockSigningAuthorityVariant BlockSigningAuthority;

    public ProducerAuthority(BinaryReader reader)
    {
        AccountName = reader.ReadName();
        BlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader);
    }
    public static ProducerAuthority ReadFromBinaryReader(BinaryReader reader)
    {
        return new ProducerAuthority(reader);
    }
}