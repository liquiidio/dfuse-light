using FASTER.common;

namespace DeepReader.Storage.Faster.Blocks.Server
{
    internal class BlockServerKeySerializer : IKeySerializer<long>
    {
        public bool Match(ref long k, bool asciiKey, ref long pattern, bool asciiPattern)
        {
            throw new NotImplementedException();
        }

        public unsafe ref long ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }
    }
}