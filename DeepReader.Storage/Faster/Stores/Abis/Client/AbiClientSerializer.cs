using DeepReader.Storage.Faster.Stores.Abis.Standalone;
using FASTER.common;

namespace DeepReader.Storage.Faster.Stores.Abis.Client
{
    internal class AbiClientSerializer : IClientSerializer<ulong, AbiCacheItem, AbiInput, AbiOutput>
    {
        public unsafe ulong ReadKey(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe AbiOutput ReadOutput(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe AbiCacheItem ReadValue(ref byte* src)
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

        public unsafe bool Write(ref AbiInput i, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }
    }
}