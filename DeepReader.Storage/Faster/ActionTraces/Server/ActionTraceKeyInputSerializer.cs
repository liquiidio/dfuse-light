using DeepReader.Storage.Faster.ActionTraces.Standalone;
using FASTER.common;

namespace DeepReader.Storage.Faster.ActionTraces.Server
{
    internal class ActionTraceKeyInputSerializer : IKeyInputSerializer<ulong, ActionTraceInput>
    {
        public unsafe ref ulong ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public bool Match(ref ulong k, bool asciiKey, ref ulong pattern, bool asciiPattern)
        {
            throw new NotImplementedException();
        }

        public unsafe ref ActionTraceInput ReadInputByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }
    }
}