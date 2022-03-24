using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerKey : IEosioSerializable<ProducerKey>
{
    public Name AccountName = Name.Empty;
    public PublicKey[] BlockSigningKey = Array.Empty<PublicKey>();//ecc.PublicKey

    public static ProducerKey ReadFromBinaryReader(BinaryReader reader)
    {
        var producerKey = new ProducerKey()
        {
            AccountName = reader.ReadName(),
        };

        producerKey.BlockSigningKey = new PublicKey[reader.Read7BitEncodedInt()];
        for (int i = 0; i < producerKey.BlockSigningKey.Length; i++)
        {
            producerKey.BlockSigningKey[i] = reader.ReadPublicKey(); //PubKeyDataSize + 1 byte for type
        }
        return producerKey;
    }
}