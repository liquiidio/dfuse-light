using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain.Legacy;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerKey : PooledObject<ProducerKey>, IEosioSerializable<ProducerKey>, IFasterSerializable<ProducerKey>
{
    public Name AccountName { get; set; }
    public PublicKey BlockSigningKey { get; set; }//ecc.PublicKey

    // TODO mandel 3.0 release will allow multiple keys and therefore will be an array,
    // current EOS and mandel have a single key instead. Probably a case for build-variable dependant builds (DEFINES injected at build time etc.?!)
    // public PublicKey[] BlockSigningKey;//ecc.PublicKey

    public ProducerKey()
    {

    }

    public static ProducerKey ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
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

    public static ProducerKey ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = fromPool ? TypeObjectPool.Get() : new ProducerKey();

        obj.AccountName = Name.ReadFromFaster(reader);

        obj.BlockSigningKey = PublicKey.ReadFromFaster(reader);
        /*BlockSigningKey = new PublicKey[reader.Read7BitEncodedInt()];
        for (var i = 0; i < BlockSigningKey.Length; i++)
        {
            BlockSigningKey[i] = reader.ReadPublicKey(); //PubKeyDataSize + 1 byte for type
        }*/
        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        AccountName.WriteToFaster(writer);
        BlockSigningKey.WriteToFaster(writer);
    }

    public new static void ReturnToPool(ProducerKey obj)
    {
        PublicKey.ReturnToPool(obj.BlockSigningKey);

        TypeObjectPool.Return(obj);
    }
}