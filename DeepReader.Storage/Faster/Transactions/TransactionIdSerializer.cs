using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions;

public class TransactionIdSerializer : BinaryObjectSerializer<TransactionId>
{
    public override void Deserialize(out TransactionId obj)
    {
        obj = new TransactionId();
    }

    public override void Serialize(ref TransactionId obj)
    {
        writer.Write(obj.Id.Binary);
    }
}