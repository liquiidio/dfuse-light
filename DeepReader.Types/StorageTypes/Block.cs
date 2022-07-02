using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Other;

namespace DeepReader.Types.StorageTypes;

public sealed class Block : PooledObject<Block>, IParentPooledObject<Block>, IFasterSerializable<Block>
{
    public Checksum256 Id { get; set; }

    public uint Number { get; set; }

    public Timestamp Timestamp { get; set; }

    public Name Producer { get; set; }

    public ushort Confirmed { get; set; }

    public Checksum256 Previous { get; set; }

    public Checksum256 TransactionMroot { get; set; }

    public Checksum256 ActionMroot { get; set; }

    public uint ScheduleVersion { get; set; }

    public ProducerSchedule? NewProducers { get; set; }

    public Signature ProducerSignature { get; set; }

    public List<TransactionId> TransactionIds { get; set; } = new();

    public List<TransactionTrace> Transactions { get; set; } = new();

    public void CopyFrom(Types.Block deepMindBlock)
    {
        Id = deepMindBlock.Id;
        Number = deepMindBlock.Number;
        Timestamp = deepMindBlock.Header.Timestamp;
        ActionMroot = deepMindBlock.Header.ActionMroot;
        Confirmed = deepMindBlock.Header.Confirmed;
        Previous = deepMindBlock.Header.Previous;
        NewProducers = deepMindBlock.Header.NewProducers;
        ScheduleVersion = deepMindBlock.Header.ScheduleVersion;
        TransactionMroot = deepMindBlock.Header.TransactionMroot;
        Producer = deepMindBlock.Header.Producer;
        ProducerSignature = deepMindBlock.ProducerSignature;
    }

    public static Block ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = TypeObjectPool.Get();

        obj.Id = Checksum256.ReadFromBinaryReader(reader);
        obj.Number = reader.ReadUInt32();
        obj.Timestamp = Timestamp.ReadFromBinaryReader(reader);
        obj.Producer = Name.ReadFromBinaryReader(reader);
        obj.Confirmed = reader.ReadUInt16();
        obj.Previous = Checksum256.ReadFromBinaryReader(reader);
        obj.TransactionMroot = Checksum256.ReadFromBinaryReader(reader);
        obj.ActionMroot = Checksum256.ReadFromBinaryReader(reader);
        obj.ScheduleVersion = reader.ReadUInt32();
        obj.ProducerSignature = Signature.ReadFromBinaryReader(reader);

        var hasNewProducers = reader.ReadByte();
        if (hasNewProducers != 0)
            obj.NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);
        else
            obj.NewProducers = null;

        var count = reader.Read7BitEncodedInt();
        for (int i = 0; i < count; i++)
        {
            obj.TransactionIds.Add(TransactionId.DeserializeKey(reader));
        }

        return obj;
    }

    public static Block ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = TypeObjectPool.Get();

        obj.Id = Checksum256.ReadFromFaster(reader);
        obj.Number = reader.ReadUInt32();
        obj.Timestamp = Timestamp.ReadFromFaster(reader);
        obj.Producer = Name.ReadFromFaster(reader);
        obj.Confirmed = reader.ReadUInt16();
        obj.Previous = Checksum256.ReadFromFaster(reader);
        obj.TransactionMroot = Checksum256.ReadFromFaster(reader);
        obj.ActionMroot = Checksum256.ReadFromFaster(reader);
        obj.ScheduleVersion = reader.ReadUInt32();
        obj.ProducerSignature = Signature.ReadFromFaster(reader);

        var hasNewProducers = reader.ReadByte();
        if (hasNewProducers != 0)
            obj.NewProducers = ProducerSchedule.ReadFromFaster(reader);
        else
            obj.NewProducers = null;

        var count = reader.Read7BitEncodedInt();
        for (int i = 0; i < count; i++)
        {
            obj.TransactionIds.Add(TransactionId.ReadFromFaster(reader));
        }

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        Id.WriteToFaster(writer);
        writer.Write(Number);
        Timestamp.WriteToFaster(writer);
        Producer.WriteToFaster(writer);
        writer.Write(Confirmed);
        Previous.WriteToFaster(writer);
        TransactionMroot.WriteToFaster(writer);
        ActionMroot.WriteToFaster(writer);
        writer.Write(ScheduleVersion);
        ProducerSignature.WriteToFaster(writer);

        writer.Write(NewProducers != null);
        if (NewProducers != null)
        {
            NewProducers.WriteToFaster(writer);
        }

        writer.Write7BitEncodedInt(TransactionIds.Count);
        foreach (var transactionId in TransactionIds)
        {
            writer.Write(transactionId.Binary);
        }

        // as we return this Object to the pool we need to reset Lists and nullables;
        NewProducers = null;
        TransactionIds.Clear();
        Transactions.Clear();
    }

    public void ReturnToPoolRecursive()
    {
        Checksum256.ReturnToPool(Id);
        Checksum256.ReturnToPool(Previous);
        Checksum256.ReturnToPool(TransactionMroot);
        Checksum256.ReturnToPool(ActionMroot);
        Timestamp.ReturnToPool(Timestamp);
        if (NewProducers != null)
        {
            ProducerSchedule.ReturnToPool(NewProducers);
            NewProducers = null;
        }
        Signature.ReturnToPool(ProducerSignature);
        foreach (var transactionId in TransactionIds)
        {
            TransactionId.ReturnToPool(transactionId);
        }
        TransactionIds.Clear();

        Transactions.Clear();

        TypeObjectPool.Return(this);
    }
}