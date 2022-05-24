﻿using System.Text.Json.Serialization;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.StorageTypes;

public class Block
{
    public Checksum256 Id { get; set; } = Checksum256.TypeEmpty;
    public uint Number { get; set; } = 0;

    public Timestamp Timestamp { get; set; }

    public Name Producer { get; set; } = Name.TypeEmpty;

    public ushort Confirmed { get; set; }

    public Checksum256 Previous { get; set; }

    public Checksum256 TransactionMroot { get; set; }

    public Checksum256 ActionMroot { get; set; }

    public uint ScheduleVersion { get; set; }

    public ProducerSchedule? NewProducers { get; set; }

    public Signature ProducerSignature { get; set; } = Signature.TypeEmpty;

    public TransactionId[] TransactionIds { get; set; } = Array.Empty<TransactionId>();

    public TransactionTrace[] Transactions { get; set; } = Array.Empty<TransactionTrace>();

    public Block()
    {

    }

    public static Block ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new Block
        {
            Id = reader.ReadChecksum256(),
            Number = reader.ReadUInt32(),
            Timestamp = reader.ReadUInt32(),
            Producer = reader.ReadName(),
            Confirmed = reader.ReadUInt16(),
            Previous = reader.ReadChecksum256(),
            TransactionMroot = reader.ReadChecksum256(),
            ActionMroot = reader.ReadChecksum256(),
            ScheduleVersion = reader.ReadUInt16(),
            ProducerSignature = reader.ReadBytes(64),
        };

        var hasNewProducers = reader.ReadBoolean();
        if (hasNewProducers)
            obj.NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);

        obj.TransactionIds = new TransactionId[reader.ReadInt32()];
        for (int i = 0; i < obj.TransactionIds.Length; i++)
        {
            obj.TransactionIds[i] = reader.ReadTransactionId();
        }

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Id.Binary);
        writer.Write(Number);
        writer.Write(Producer.Binary);
        writer.Write(ProducerSignature.Binary);
        writer.Write(TransactionIds.Length);
        foreach (var transactionId in TransactionIds)
        {
            writer.Write(transactionId.Binary);
        }
    }
}