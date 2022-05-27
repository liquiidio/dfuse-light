using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerKey : PooledObject<ProducerKey>, IEosioSerializable<ProducerKey>
{
    public Name AccountName;
    public PublicKey BlockSigningKey;//ecc.PublicKey

    // TODO mandel 3.0 release will allow multiple keys and therefore will be an array,
    // current EOS and mandel have a single key instead. Probably a case for build-variable dependant builds (DEFINES injected at build time etc.?!)
    // public PublicKey[] BlockSigningKey;//ecc.PublicKey

    public ProducerKey()
    {

    }
    public static ProducerKey ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new ProducerKey();

        obj.AccountName = Name.ReadFromBinaryReader(reader);

        obj.BlockSigningKey = PublicKey.ReadFromBinaryReader(reader);
        /*BlockSigningKey = new PublicKey[reader.Read7BitEncodedInt()];
        for (var i = 0; i < BlockSigningKey.Length; i++)
        {
            BlockSigningKey[i] = reader.ReadPublicKey(); //PubKeyDataSize + 1 byte for type
        }*/
        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        AccountName.WriteToBinaryWriter(writer);
        BlockSigningKey.WriteToBinaryWriter(writer);

        TypeObjectPool.Return(this);
    }
}