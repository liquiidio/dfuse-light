using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions;

public class TransactionValueSerializer : BinaryObjectSerializer<FlattenedTransactionTrace>
{
    public override void Deserialize(out FlattenedTransactionTrace obj)
    {
        obj = FlattenedTransactionTrace.ReadFromBinaryReader(reader);
    }

    public override void Serialize(ref FlattenedTransactionTrace obj)
    {
        obj.WriteToBinaryWriter(writer);
    }
}