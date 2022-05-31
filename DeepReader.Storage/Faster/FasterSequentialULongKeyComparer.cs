using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FASTER.core;

namespace DeepReader.Storage.Faster
{
    internal class FasterSequentialLongKeyComparer : IFasterEqualityComparer<long>
    {
        public long GetHashCode64(ref long k)
        {
            return k;
        }

        public bool Equals(ref long k1, ref long k2)
        {
            return k1 == k2;
        }
    }
}
