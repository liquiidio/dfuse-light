using System.Runtime.CompilerServices;
using FASTER.core;

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

        public unsafe long GetHashCode64(ref TransactionId id)
        {
            byte* ptr = (byte*)Unsafe.AsPointer(ref id.Id.Binary);
            return Utility.HashBytes(ptr, id.Id.Binary.Length);
        }

        public bool Equals(ref TransactionId k1, ref TransactionId k2)
        {
            return k1.Id.Binary.SequenceEqual(k2.Id.Binary);
        }
    }
}
