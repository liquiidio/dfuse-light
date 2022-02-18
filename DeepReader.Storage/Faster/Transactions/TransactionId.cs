using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FASTER.core;
using Microsoft.VisualBasic.CompilerServices;

namespace DeepReader.Storage.Faster.Transactions
{
    public class TransactionId : IFasterEqualityComparer<TransactionId>
    {
        public Types.Eosio.Chain.TransactionId Id = Types.Eosio.Chain.TransactionId.Empty;

        public TransactionId()
        {

        }

        public TransactionId(Types.Eosio.Chain.TransactionId id)
        {
            Id = id;
        }

        public long GetHashCode64(ref TransactionId id)
        {

            return Id.Binary.Length >= 8 ? BitConverter.ToInt64(Id.Binary.Take(8).ToArray()) : 0 ;
        }

        public bool Equals(ref TransactionId k1, ref TransactionId k2)
        {
            return k1.Id.Binary == k2.Id.Binary;
        }
    }
}
