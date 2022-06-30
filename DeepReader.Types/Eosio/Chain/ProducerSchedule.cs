using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Extensions;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerSchedule : PooledObject<ProducerSchedule>, IEosioSerializable<ProducerSchedule>, IFasterSerializable<ProducerSchedule>
{
    public uint Version { get; set; }//uint32
    public ProducerKey[] Producers { get; set; }//[]*ProducerKey

    public ProducerSchedule()
    {
        Producers = Array.Empty<ProducerKey>();
    }

    public static ProducerSchedule ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = fromPool ? TypeObjectPool.Get() : new ProducerSchedule();

        obj.Version = reader.ReadUInt32();

        obj.Producers = new ProducerKey[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Producers.Length; i++)
        {
            obj.Producers[i] = ProducerKey.ReadFromBinaryReader(reader);
        }

        return obj;
    }

    public static ProducerSchedule ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = fromPool ? TypeObjectPool.Get() : new ProducerSchedule();

        obj.Version = reader.ReadUInt32();

        obj.Producers = new ProducerKey[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Producers.Length; i++)
        {
            obj.Producers[i] = ProducerKey.ReadFromFaster(reader);
        }

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write(Version);

        writer.Write7BitEncodedInt(Producers.Length);
        foreach (var producer in Producers)
        {
            producer.WriteToFaster(writer);
        }
    }

    public new static void ReturnToPool(ProducerSchedule obj)
    {
        foreach (var producerKey in obj.Producers)
        {
            ProducerKey.ReturnToPool(producerKey);
        }
        obj.Producers = Array.Empty<ProducerKey>();

        TypeObjectPool.Return(obj);
    }
}