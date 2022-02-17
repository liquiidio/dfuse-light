using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions;

public sealed class TransactionFunctions : FunctionsBase<TransactionId, FlattenedTransactionTrace, TransactionInput,
    TransactionOutput, TransactionContext>
{
    public override void ConcurrentReader(ref TransactionId id, ref TransactionInput input,
        ref FlattenedTransactionTrace value, ref TransactionOutput dst)
    {
        dst.Value = value;
    }

    public override void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
    {
        Console.WriteLine("Session {0} reports persistence until {1}", sessionId, commitPoint.UntilSerialNo);
    }

    public override void ReadCompletionCallback(ref TransactionId id, ref TransactionInput input,
        ref TransactionOutput output, TransactionContext ctx, Status status)
    {
        if (ctx.Type == 0)
        {
            if (output.Value.Id != id.Id)
                throw new Exception("Read error!");
        }
        else
        {
            long ticks = DateTime.Now.Ticks - ctx.Ticks;

            if (status == Status.NOTFOUND)
                Console.WriteLine("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

            if (output.Value.Id != id.Id)
                Console.WriteLine("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Id,
                    new TimeSpan(ticks).TotalMilliseconds);
            else
                Console.WriteLine("Async: Correct value {0} found, latency = {1}ms", output.Value.Id,
                    new TimeSpan(ticks).TotalMilliseconds);
        }
    }

    public override void SingleReader(ref TransactionId id, ref TransactionInput input,
        ref FlattenedTransactionTrace value, ref TransactionOutput dst)
    {
        dst.Value = value;
    }
}