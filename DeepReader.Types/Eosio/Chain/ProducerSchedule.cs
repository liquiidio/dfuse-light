using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Other;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/producer_schedule.hpp
/// </summary>
public sealed class ProducerSchedule : PooledObject<ProducerSchedule>, IEosioSerializable<ProducerSchedule>
{
    public uint Version { get; set; }//uint32
    public ProducerKey[] Producers { get; set; }//[]*ProducerKey

    public ProducerSchedule()
    {
        Producers = Array.Empty<ProducerKey>();
    }

    public static ProducerSchedule ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
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

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Version);

        writer.Write7BitEncodedInt(Producers.Length);
        foreach (var producer in Producers)
        {
            producer.WriteToBinaryWriter(writer);
        }

        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        TypeObjectPool.Return(this);
    }
}