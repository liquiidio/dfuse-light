using FASTER.core;

namespace DeepReader.Storage.Faster.Abis
{
    public sealed class AbiId : IFasterEqualityComparer<AbiId>
    {
        public ulong Id;

        public AbiId(ulong id)
        {
            Id = id;
        }

        public long GetHashCode64(ref AbiId id)
        {
            return (long)id.Id;
        }

        public bool Equals(ref AbiId k1, ref AbiId k2)
        {
            return k1.Id == k2.Id;
        }
    }
}
