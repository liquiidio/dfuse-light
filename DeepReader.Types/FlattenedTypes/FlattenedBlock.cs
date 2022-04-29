﻿using System.Text.Json.Serialization;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.FlattenedTypes;

public struct FlattenedBlock
{
    public Checksum256 Id = Checksum256.Empty;
    public uint Number = 0;

    public Name Producer = Name.Empty;
    public Signature ProducerSignature = Signature.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]   // TODO (Corvin) not sure if this works for Collections
    public TransactionId[] TransactionIds = Array.Empty<TransactionId>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]   // TODO (Corvin) not sure if this works for Collections
    public FlattenedTransactionTrace[] Transactions = Array.Empty<FlattenedTransactionTrace>();

    public FlattenedBlock()
    {

    }

    public static FlattenedBlock ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new FlattenedBlock
        {
            Id = reader.ReadBytes(32),
            Number = reader.ReadUInt16(),
            Producer = reader.ReadName(),
            ProducerSignature = reader.ReadBytes(64),
        };

        obj.TransactionIds = new TransactionId[reader.ReadInt32()];
        for (int i = 0; i < obj.TransactionIds.Length; i++)
        {
            obj.TransactionIds[i] = reader.ReadBytes(32);
        }

        // Ignore TransactionTraces here!

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