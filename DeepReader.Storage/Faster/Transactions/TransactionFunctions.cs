using DeepReader.Types.StorageTypes;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Transactions;

public sealed class TransactionFunctions : FunctionsBase<TransactionId, TransactionTrace, TransactionInput,TransactionOutput, TransactionContext>
{
    public override bool ConcurrentReader(ref TransactionId id, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
    {
        Log.Information("Session {0} reports persistence until {1}", sessionName, commitPoint.UntilSerialNo);
    }

    public override void ReadCompletionCallback(ref TransactionId id, ref TransactionInput input, ref TransactionOutput output, TransactionContext ctx, Status status, RecordMetadata recordMetadata)
    {
        if (ctx.Type == 0)
        {
            if (!output.Value.Id.Binary.SequenceEqual(id.Id.Binary))
                Log.Error( new Exception("Read error!, TransactionId.BinarySequencens unequal"),"");
        }
        else
        {
            long ticks = DateTime.Now.Ticks - ctx.Ticks;

            if (status.Found)
                Log.Information("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

            if (output.Value.Id != id.Id)
                Log.Information("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Id,
                    new TimeSpan(ticks).TotalMilliseconds);
            else
                Log.Information("Async: Correct value {0} found, latency = {1}ms", output.Value.Id,
                    new TimeSpan(ticks).TotalMilliseconds);
        }
    }

    public override bool SingleReader(ref TransactionId id, ref TransactionInput input, ref TransactionTrace value, ref TransactionOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }
}