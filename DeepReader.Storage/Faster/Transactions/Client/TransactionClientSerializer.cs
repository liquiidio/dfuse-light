using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Client
{
    internal class TransactionClientSerializer : IClientSerializer<TransactionId, TransactionTrace, TransactionInput, TransactionOutput>
    {
        public unsafe TransactionId ReadKey(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe TransactionOutput ReadOutput(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe TransactionTrace ReadValue(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref TransactionId k, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref TransactionTrace v, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }

        public unsafe bool Write(ref TransactionInput i, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }
    }
}