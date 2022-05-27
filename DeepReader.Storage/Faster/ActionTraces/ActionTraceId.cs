﻿using FASTER.core;

namespace DeepReader.Storage.Faster.ActionTraces
{
    public sealed class ActionTraceId : IFasterEqualityComparer<ActionTraceId>
    {
        public ulong GlobalSequence = 0;

        public ActionTraceId(ulong globalSequence)
        {
            GlobalSequence = globalSequence;
        }

        public long GetHashCode64(ref ActionTraceId id)
        {

            return (long)id.GlobalSequence;
        }

        public bool Equals(ref ActionTraceId k1, ref ActionTraceId k2)
        {
            return  k1.GlobalSequence == k2.GlobalSequence;
        }
    }
}
