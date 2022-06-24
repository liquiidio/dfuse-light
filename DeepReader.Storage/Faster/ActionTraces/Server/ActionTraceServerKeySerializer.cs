using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FASTER.common;

namespace DeepReader.Storage.Faster.ActionTraces.Server
{
    internal class ActionTraceServerKeySerializer : IKeySerializer<ulong>
    {
        public unsafe ref ulong ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public bool Match(ref ulong k, bool asciiKey, ref ulong pattern, bool asciiPattern)
        {
            throw new NotImplementedException();
        }
    }
}
