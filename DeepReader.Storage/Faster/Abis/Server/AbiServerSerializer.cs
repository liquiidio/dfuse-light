using DeepReader.Storage.Faster.Abis.Standalone;
using FASTER.common;

namespace DeepReader.Storage.Faster.Abis.Server
{
    internal class AbiServerSerializer : IServerSerializer<ulong, AbiCacheItem, AbiInput, AbiOutput>
    {
        public unsafe ref AbiOutput AsRefOutput(byte* src, int length)
        {
            throw new NotImplementedException();
        }

        public int GetLength(ref AbiOutput o)
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

        public unsafe ref AbiCacheItem ReadValueByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe void SkipOutput(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref ulong k, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref AbiCacheItem v, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref AbiOutput o, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }
    }
}