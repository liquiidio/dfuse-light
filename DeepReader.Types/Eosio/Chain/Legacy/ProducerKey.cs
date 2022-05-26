using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerKey : IEosioSerializable<ProducerKey>
{
    public Name AccountName;
    public PublicKey BlockSigningKey;//ecc.PublicKey

    // TODO mandel 3.0 release will allow multiple keys and therefore will be an array,
    // current EOS and mandel have a single key instead. Probably a case for build-variable dependant builds (DEFINES injected at build time etc.?!)
    // public PublicKey[] BlockSigningKey;//ecc.PublicKey

    public ProducerKey(BinaryReader reader)
    {
        AccountName = reader.ReadName();

        BlockSigningKey = reader.ReadPublicKey();
        /*BlockSigningKey = new PublicKey[reader.Read7BitEncodedInt()];
        for (var i = 0; i < BlockSigningKey.Length; i++)
        {
            BlockSigningKey[i] = reader.ReadPublicKey(); //PubKeyDataSize + 1 byte for type
        }*/
    }
    public static ProducerKey ReadFromBinaryReader(BinaryReader reader)
    {
        return new ProducerKey(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(AccountName);
        writer.Write(BlockSigningKey.Binary);
    }
}