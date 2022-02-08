using FASTER.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.StorageAdapters.Faster
{
    public class CacheKey : IFasterEqualityComparer<CacheKey>
    {
        public long Key;

        public CacheKey()
        {

        }

        public CacheKey(long first)
        {
            Key = first;
        }

        public long GetHashCode64(ref CacheKey key)
        {
            return Utility.GetHashCode(key.Key);
        }
        public bool Equals(ref CacheKey k1, ref CacheKey k2)
        {
            return k1.Key == k2.Key;
        }
    }

    public class CacheKeySerializer : BinaryObjectSerializer<CacheKey>
    {
        public override void Deserialize(out CacheKey obj)
        {
            obj = new CacheKey(reader.ReadInt64());
        }

        public override void Serialize(ref CacheKey obj)
        {
            writer.Write(obj.Key);
        }
    }

    public class CacheValue
    {
        public long Value;

        public CacheValue()
        {

        }

        public CacheValue(long first)
        {
            Value = first;
        }
    }

    public class CacheValueSerializer : BinaryObjectSerializer<CacheValue>
    {
        public override void Deserialize(out CacheValue obj)
        {
            obj = new CacheValue(reader.ReadInt64());
        }

        public override void Serialize(ref CacheValue obj)
        {
            writer.Write(obj.Value);
        }
    }

    public struct CacheInput
    {
    }

    public struct CacheOutput
    {
        public CacheValue Value;
    }

    public struct CacheContext
    {
        public int Type;
        public long Ticks;
    }

    public sealed class CacheFunctions : FunctionsBase<CacheKey, CacheValue, CacheInput, CacheOutput, CacheContext>
    {
        public override void ConcurrentReader(ref CacheKey key, ref CacheInput input, ref CacheValue value, ref CacheOutput dst)
        {
            dst.Value = value;
        }

        public override void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
        {
            // Console.WriteLine("Session {0} reports persistence until {1}", sessionId, commitPoint.UntilSerialNo);
        }

        public override void ReadCompletionCallback(ref CacheKey key, ref CacheInput input, ref CacheOutput output, CacheContext ctx, Status status)
        {
            if (ctx.Type == 0)
            {
                if (output.Value.Value != key.Key)
                    throw new Exception("Read error!");
            }
            else
            {
                long ticks = DateTime.Now.Ticks - ctx.Ticks;

                if (status == Status.NOTFOUND)
                    Console.WriteLine("Async: Value not found, latency = {0}ms", new TimeSpan(ticks).TotalMilliseconds);

                if (output.Value.Value != key.Key)
                    Console.WriteLine("Async: Incorrect value {0} found, latency = {1}ms", output.Value.Value, new TimeSpan(ticks).TotalMilliseconds);
                else
                    Console.WriteLine("Async: Correct value {0} found, latency = {1}ms", output.Value.Value, new TimeSpan(ticks).TotalMilliseconds);
            }
        }

        public override void SingleReader(ref CacheKey key, ref CacheInput input, ref CacheValue value, ref CacheOutput dst)
        {
            dst.Value = value;
        }
    }
}
