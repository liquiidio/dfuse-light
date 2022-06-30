using DeepReader.Types.StorageTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions.Standalone;

public sealed class TransactionValueSerializer : BinaryObjectSerializer<TransactionTrace>
{
    public override void Deserialize(out TransactionTrace obj)
    {
        obj = TransactionTrace.ReadFromFaster(reader);
    }

    public override void Serialize(ref TransactionTrace obj)
    {
        obj.WriteToFaster(writer);
    }
}