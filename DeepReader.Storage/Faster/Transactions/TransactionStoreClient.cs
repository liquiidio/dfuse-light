using DeepReader.Types.StorageTypes;
using FASTER.client;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DeepReader.Storage.Faster.Transactions
{
    internal class TransactionStoreClient
    {
        private const string ip = "127.0.0.1";
        private const int port = 5003;
        private static Encoding _encode = Encoding.UTF8;
        private static ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

        private readonly FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> _client;

        private ClientSession<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, (IMemoryOwner<byte>, int), byte, TransactionMemoryFunctions, MemoryParameterSerializer<byte>> _session;

        public TransactionStoreClient()
        {
            _client = new FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(ip, port);
            _session = _client.NewSession(new TransactionMemoryFunctions());
        }

        public async Task WriteTransaction(TransactionTrace transaction)
        {
            Debug.WriteLine("Write Transaction");
            int transactionIdLength = _encode.GetByteCount(transaction.Id.ToString());
            int blockLength = _encode.GetByteCount(JsonSerializer.Serialize(transaction));

            byte[] transactionIdBytes = _pool.Rent(transactionIdLength);
            int bytesWritten = _encode.GetBytes(transaction.Id.ToString(), transactionIdBytes);
            var key = transactionIdBytes.AsMemory(0, bytesWritten);

            byte[] transactionBytes = _pool.Rent(blockLength);
            bytesWritten = _encode.GetBytes(JsonSerializer.Serialize(transaction), transactionBytes);
            var value = transactionBytes.AsMemory(0, bytesWritten);

            await _session.UpsertAsync(key, value);

            _session.CompletePending();
        }

        public async Task<(bool, TransactionTrace)> TryGetTransactionTraceById(Types.Eosio.Chain.TransactionId transactionId)
        {
            int transactionIdLength = _encode.GetByteCount(transactionId.StringVal);

            byte[] transactionIdBytes = _pool.Rent(transactionIdLength);
            int bytesWritten = _encode.GetBytes(transactionId.StringVal, transactionIdBytes);
            var key = transactionIdBytes.AsMemory(0, bytesWritten);

            var (status, output) = await _session.ReadAsync(key);

            var transaction = JsonSerializer.Deserialize<TransactionTrace>(output.Item1.Memory.Span.Slice(0, output.Item2));
            return (status.Found, transaction);
        }
    }
}