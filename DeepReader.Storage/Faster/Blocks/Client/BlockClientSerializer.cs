using DeepReader.Storage.Faster.Blocks.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Blocks.Client
{
    internal class BlockClientSerializer : IClientSerializer<long, Block, BlockInput, BlockOutput>
    {
        public unsafe long ReadKey(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe BlockOutput ReadOutput(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe Block ReadValue(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref long k, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref Block v, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref BlockInput i, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }
    }
}