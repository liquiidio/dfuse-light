using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerKey : IEosioSerializable<ProducerKey>
{
    public Name AccountName;
    public PublicKey[] BlockSigningKey;//ecc.PublicKey

    public ProducerKey(BinaryReader reader)
    {
        AccountName = reader.ReadName();

        BlockSigningKey = new PublicKey[reader.Read7BitEncodedInt()];
        for (var i = 0; i < BlockSigningKey.Length; i++)
        {
            BlockSigningKey[i] = reader.ReadPublicKey(); //PubKeyDataSize + 1 byte for type
        }
    }
    public static ProducerKey ReadFromBinaryReader(BinaryReader reader)
    {
        return new ProducerKey(reader);
    }
}