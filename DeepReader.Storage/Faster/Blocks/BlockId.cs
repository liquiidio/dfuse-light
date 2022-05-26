﻿using FASTER.core;

namespace DeepReader.Storage.Faster.Blocks
{
    public sealed class BlockId : IFasterEqualityComparer<BlockId>
    {
        public long Id;

        public BlockId()
        {

        }

        public BlockId(long id)
        {
            Id = id;
        }

        public long GetHashCode64(ref BlockId id)
        {
            return id.Id;
        }
        public bool Equals(ref BlockId k1, ref BlockId k2)
        {
            return k1.Id == k2.Id;
        }
    }
}
