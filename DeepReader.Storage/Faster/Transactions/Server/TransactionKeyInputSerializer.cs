using DeepReader.Storage.Faster.Transactions.Standalone;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionKeyInputSerializer : IKeyInputSerializer<TransactionId, TransactionInput>
    {
        public bool Match(ref TransactionId k, bool asciiKey, ref TransactionId pattern, bool asciiPattern)
        {
            throw new NotImplementedException();
        }

        public unsafe ref TransactionInput ReadInputByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe ref TransactionId ReadKeyByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }
    }
}