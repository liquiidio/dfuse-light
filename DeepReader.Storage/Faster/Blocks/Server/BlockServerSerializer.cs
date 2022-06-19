using DeepReader.Storage.Faster.Blocks.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Blocks.Server
{
    internal class BlockServerSerializer : IServerSerializer<long, Block, BlockInput, BlockOutput>
    {
        public unsafe ref BlockOutput AsRefOutput(byte* src, int length)
        {
            throw new NotImplementedException();
        }

        public int GetLength(ref BlockOutput o)
        {
            throw new NotImplementedException();
        }

        public unsafe ref BlockInput ReadInputByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe ref long ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe ref Block ReadValueByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe void SkipOutput(ref byte* src)
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

        public unsafe bool Write(ref BlockOutput o, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }
    }
}