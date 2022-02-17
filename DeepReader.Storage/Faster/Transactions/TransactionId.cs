using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FASTER.core;

namespace DeepReader.Storage.Faster.Transactions
{
    public class TransactionId : IFasterEqualityComparer<TransactionId>
    {
        public byte[] Id;

        public TransactionId()
        {

        }

        public TransactionId(byte[] id)
        {
            Id = id;
        }

        public long GetHashCode64(ref TransactionId id)
        {
            return id.Id.GetHashCode();
        }

        public bool Equals(ref TransactionId k1, ref TransactionId k2)
        {
            return k1.Id == k2.Id;
        }
    }
}
