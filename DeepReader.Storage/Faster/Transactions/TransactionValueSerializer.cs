﻿using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions;

public class TransactionValueSerializer : BinaryObjectSerializer<TransactionTrace>
{
    public override void Deserialize(out TransactionTrace obj)
    {
        obj = TransactionTrace.ReadFromBinaryReader(reader);
    }

    public override void Serialize(ref TransactionTrace obj)
    {
        obj.WriteToBinaryWriter(writer);
    }
}