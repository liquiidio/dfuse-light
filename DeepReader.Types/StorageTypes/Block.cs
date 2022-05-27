using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Other;

namespace DeepReader.Types.StorageTypes;

public sealed class Block : PooledObject<Block>, IParentPooledObject<Block>
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

    /// <summary>
    /// Called in cases of a failure while postprocessing blocks. Ensures the block and all other Pooled objects will be returned to the pool
    /// </summary>
    public void ReturnToPoolRecursive()
    {
        Checksum256.ReturnToPool(Id);
        Checksum256.ReturnToPool(Previous);
        Checksum256.ReturnToPool(TransactionMroot);
        Checksum256.ReturnToPool(ActionMroot);
        Timestamp.ReturnToPool(Timestamp);
        if(NewProducers != null)
            ProducerSchedule.ReturnToPool(NewProducers);
        Signature.ReturnToPool(ProducerSignature);
        foreach (var transactionId in TransactionIds)
        {
            TransactionId.ReturnToPool(transactionId);
        }

        foreach (var transactionTrace in Transactions)
        {
            TransactionTrace.ReturnToPool(transactionTrace);
        }

        TypeObjectPool.Return(this);
    }

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

    public static Block ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
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

        var hasNewProducers = reader.ReadBoolean();
        if (hasNewProducers)
            obj.NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);
        else
            obj.NewProducers = null;

        for (int i = 0; i < reader.ReadInt32(); i++)
        {
            obj.TransactionIds.Add(TransactionId.ReadFromBinaryReader(reader));
        }

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        Id.WriteToBinaryWriter(writer);
        writer.Write(Number);
        Timestamp.WriteToBinaryWriter(writer);
        Producer.WriteToBinaryWriter(writer);
        writer.Write(Confirmed);
        Previous.WriteToBinaryWriter(writer);
        TransactionMroot.WriteToBinaryWriter(writer);
        ActionMroot.WriteToBinaryWriter(writer);
        writer.Write(ScheduleVersion);
        ProducerSignature.WriteToBinaryWriter(writer);
        
        writer.Write(NewProducers != null);
        if (NewProducers != null)
        {
            NewProducers.WriteToBinaryWriter(writer);
        }

        writer.Write(TransactionIds.Count);
        foreach (var transactionId in TransactionIds)
        {
            writer.Write(transactionId.Binary);
        }

        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        TypeObjectPool.Return(this);
    }
}