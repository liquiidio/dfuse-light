using DeepReader.Storage.Faster.Abis.Standalone;
using FASTER.common;

namespace DeepReader.Storage.Faster.Abis.Server
{
    internal class AbiKeyInputSerializer : IKeyInputSerializer<ulong, AbiInput>
    {
        public bool Match(ref ulong k, bool asciiKey, ref ulong pattern, bool asciiPattern)
        {
            throw new NotImplementedException();
        }

        public unsafe ref AbiInput ReadInputByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe ref ulong ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }
    }
}