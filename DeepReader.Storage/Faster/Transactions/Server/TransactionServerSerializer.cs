using DeepReader.Storage.Faster.Transactions.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;

namespace DeepReader.Storage.Faster.Transactions.Server
{
    internal class TransactionServerSerializer : IServerSerializer<TransactionId, TransactionTrace, TransactionInput, TransactionOutput>
    {
        public unsafe ref TransactionOutput AsRefOutput(byte* src, int length)
        {
            throw new NotImplementedException();
        }

        public int GetLength(ref TransactionOutput o)
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

        public unsafe ref TransactionTrace ReadValueByRef(ref byte* src)
        {
            throw new NotImplementedException();
        }

        public unsafe void SkipOutput(ref byte* src)
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

        public unsafe bool Write(ref TransactionOutput o, ref byte* dst, int length)
        {
            throw new NotImplementedException();
        }
    }
}