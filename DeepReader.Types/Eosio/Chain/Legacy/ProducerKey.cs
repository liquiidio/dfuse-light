using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain.Legacy;

// Todo: @corvin we need to update this url

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public class ProducerKey
{
    public Name AccountName = Name.Empty;
    public PublicKey[] BlockSigningKey = Array.Empty<PublicKey>();//ecc.PublicKey

    public static ProducerKey ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new ProducerKey()
        {
            AccountName = reader.ReadUInt64(),
        };

        obj.BlockSigningKey = new PublicKey[reader.ReadInt32()];
        for (int i = 0; i < obj.BlockSigningKey.Length; i++)
        {
            obj.BlockSigningKey[i] = reader.ReadString();
        }
        return obj;
    }
}