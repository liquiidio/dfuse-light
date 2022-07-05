﻿using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using DeepReader.Types.Other;

namespace DeepReader.Types.StorageTypes;

public sealed class TransactionTrace : PooledObject<TransactionTrace>, IParentPooledObject<TransactionTrace>, IEosioSerializable<TransactionTrace>, IFasterSerializable<TransactionTrace>
{
    // SHA-256 (FIPS 180-4) of the FCBUFFER-encoded packed transaction
    public TransactionId Id { get; set; } = Array.Empty<byte>();

    public uint BlockNum { get; set; } = 0;
    // Status

    public TransactionReceiptHeader Receipt { get; set; }

    public long Elapsed { get; set; } = 0;

    public ulong NetUsage { get; set; } = 0;

    public bool Scheduled { get; set; } = false;

    public ActionTrace[] ActionTraces { get; set; } = Array.Empty<ActionTrace>();

    public ulong[] ActionTraceIds { get; set; } = Array.Empty<ulong>();

    public TransactionTrace()
    {

    }

    public void CopyFrom(Eosio.Chain.TransactionTrace transactionTrace)
    {
        Id = transactionTrace.Id;
        BlockNum = transactionTrace.BlockNum;
        Receipt = transactionTrace.Receipt!;
        Elapsed = transactionTrace.Elapsed;
        NetUsage = transactionTrace.NetUsage;
        Scheduled = transactionTrace.Scheduled;
    }

    public static TransactionTrace ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = TypeObjectPool.Get();

        obj.Id = reader.ReadBytes(32);
        obj.BlockNum = reader.ReadUInt32();
        obj.Elapsed = reader.ReadInt64();
        obj.NetUsage = reader.ReadUInt64();
        obj.Scheduled = reader.ReadBoolean();

        obj.Receipt = TransactionReceiptHeader.ReadFromBinaryReader(reader);

        obj.ActionTraceIds = new ulong[reader.ReadInt32()];
        for (int i = 0; i < obj.ActionTraceIds.Length; i++)
        {
            obj.ActionTraceIds[i] = reader.ReadUInt64();
        }

        return obj;
    }

    public static TransactionTrace ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = TypeObjectPool.Get();

        obj.Id = reader.ReadBytes(32);
        obj.BlockNum = reader.ReadUInt32();
        obj.Elapsed = reader.ReadInt64();
        obj.NetUsage = reader.ReadUInt64();
        obj.Scheduled = reader.ReadBoolean();

        obj.Receipt = TransactionReceiptHeader.ReadFromFaster(reader);

        obj.ActionTraceIds = new ulong[reader.ReadInt32()];
        for (int i = 0; i < obj.ActionTraceIds.Length; i++)
        {
            obj.ActionTraceIds[i] = reader.ReadUInt64();
        }

        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        Id.WriteToFaster(writer);

        writer.Write(BlockNum); // TODO VARINT
        writer.Write(Elapsed); // TODO VARINT
        writer.Write(NetUsage); // TODO VARINT
        writer.Write(Scheduled); // TODO VARINT

        Receipt.WriteToFaster(writer);

        writer.Write(ActionTraceIds.Length);
        foreach (var actionTraceId in ActionTraceIds)
        {
            writer.Write(actionTraceId);
        }
    }

    public void ReturnToPoolRecursive()
    {
        TransactionId.ReturnToPool(Id);
        //        TransactionReceiptHeader Receipt

        ActionTraces = Array.Empty<ActionTrace>();

        TypeObjectPool.Return(this);
    }
}