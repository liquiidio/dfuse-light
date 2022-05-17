using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.EosTypes;
using FASTER.core;
using Microsoft.VisualBasic.CompilerServices;

namespace DeepReader.Storage.Faster.Abis
{
    public class AbiId : IFasterEqualityComparer<AbiId>
    {
        public ulong Id;

        public AbiId()
        {

        }

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
