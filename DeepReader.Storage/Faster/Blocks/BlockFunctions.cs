﻿using DeepReader.Types.FlattenedTypes;
using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks;

public sealed class BlockFunctions : FunctionsBase<BlockId, FlattenedBlock, BlockInput, BlockOutput, BlockContext>
{
    public override bool ConcurrentReader(ref BlockId id, ref BlockInput input, ref FlattenedBlock value, ref BlockOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }

    public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
    {
        Console.WriteLine("Session {0} reports persistence until {1}", sessionId, commitPoint.UntilSerialNo);
    }

    public override void ReadCompletionCallback(ref BlockId id, ref BlockInput input, ref BlockOutput output, BlockContext ctx, Status status, RecordMetadata recordMetadata)
    {
        if (ctx.Type == 0)
        {
            if (output.Value.Number != id.Id)
                throw new Exception("Read error!");
        }
        else
        {
            long ticks = DateTime.Now.Ticks - ctx.Ticks;

            if (status.Found)
                Console.WriteLine("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

            if (output.Value.Number != id.Id)
                Console.WriteLine("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Number, new TimeSpan(ticks).TotalMilliseconds);
            else
                Console.WriteLine("Async: Correct value {0} found, latency = {1}ms", output.Value.Number, new TimeSpan(ticks).TotalMilliseconds);
        }
    }

    public override bool SingleReader(ref BlockId id, ref BlockInput input, ref FlattenedBlock value, ref BlockOutput dst, ref ReadInfo readInfo)
    {
        dst.Value = value;
        return true;
    }
}