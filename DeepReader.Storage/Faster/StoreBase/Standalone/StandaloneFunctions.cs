using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Storage.Faster.Test;
using FASTER.core;
using Serilog;

namespace DeepReader.Storage.Faster.Base.Standalone
{
    internal class
        StandaloneFunctions<TKey, TValue> : FunctionsBase<TKey, TValue, Input, TValue, KeyValueContext>
    {
        public override bool ConcurrentReader(ref TKey id, ref Input input, ref TValue value, ref TValue dst, ref ReadInfo readInfo)
        {
            dst = value;
            return true;
        }

        public override void CheckpointCompletionCallback(int sessionId, string sessionName, CommitPoint commitPoint)
        {
            Log.Information("Session {0} reports persistence until {1}", sessionName, commitPoint.UntilSerialNo);
        }

        public override bool SingleReader(ref TKey id, ref Input input, ref TValue value, ref TValue dst, ref ReadInfo readInfo)
        {
            dst = value;
            return true;
        }
    }
}
