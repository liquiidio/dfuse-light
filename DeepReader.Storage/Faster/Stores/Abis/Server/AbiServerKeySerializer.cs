using FASTER.common;

namespace DeepReader.Storage.Faster.Abis.Server
{
    internal class AbiServerKeySerializer : IKeySerializer<ulong>
    {
        public bool Match(ref ulong k, bool asciiKey, ref ulong pattern, bool asciiPattern)
        {
            throw new NotImplementedException();
        }

        public unsafe ref ulong ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }
    }
}