using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerAuthority
{
    public Name AccountName = Name.Empty;
    public BlockSigningAuthorityVariant BlockSigningAuthority = new BlockSigningAuthorityV0();

    public static ProducerAuthority ReadFromBinaryReader(BinaryReader reader)
    {
        var producerAuthority = new ProducerAuthority()
        {
            AccountName = reader.ReadName(),

            // Todo: (Haron) Check on this variant
            // Corvin: "Already did that and changed BlockSigningAuthorityV0.ReadFromBinaryReader to BlockSigningAuthorityVariant.ReadFromBinaryReader"
            BlockSigningAuthority = BlockSigningAuthorityVariant.ReadFromBinaryReader(reader)
        };
        return producerAuthority;
    }
}