using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FASTER.core;

namespace DeepReader.Storage.Faster
{
    internal class FasterSequentialULongKeyComparer : IFasterEqualityComparer<ulong>
    {
        public long GetHashCode64(ref ulong k)
        {
            return (long)k;
        }

        public bool Equals(ref ulong k1, ref ulong k2)
        {
            return k1 == k2;
        }
    }
}
