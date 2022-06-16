using DeepReader.Types.StorageTypes;
using FASTER.client;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DeepReader.Storage.Faster.Blocks
{
    internal class BlockStoreClient
    {
        private const string ip = "127.0.0.1";
        private const int port = 5002;
        private static Encoding _encode = Encoding.UTF8;
        private static ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

        private readonly FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> _client;
        
        private ClientSession<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, (IMemoryOwner<byte>, int), byte, BlockMemoryFunctions, MemoryParameterSerializer<byte>> _session;

        //private readonly AsyncPool<ClientSession<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, (IMemoryOwner<byte>, int), byte, BlockMemoryFunctions, MemoryParameterSerializer<byte>>> _sessionPool;

        public BlockStoreClient()
        {
            _client = new FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(ip, port);
            _session = _client.NewSession(new BlockMemoryFunctions());

            //_sessionPool = new AsyncPool<ClientSession<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, (IMemoryOwner<byte>, int), byte, BlockMemoryFunctions, MemoryParameterSerializer<byte>>>(10, () => _client.NewSession(new BlockMemoryFunctions()));
        }

        public async Task WriteBlock(Block block)
        {
            Debug.WriteLine($"{block.Number} has been writtten");
            int blockIdLength = _encode.GetByteCount(block.Number.ToString());
            int blockLength = _encode.GetByteCount(JsonSerializer.Serialize(block));

            byte[] blockIdBytes = _pool.Rent(blockIdLength);
            int bytesWritten = _encode.GetBytes(block.Number.ToString(), blockIdBytes);
            var key = blockIdBytes.AsMemory(0, bytesWritten);

            byte[] blockBytes = _pool.Rent(blockLength);
            bytesWritten = _encode.GetBytes(JsonSerializer.Serialize(block), blockBytes);
            var value = blockBytes.AsMemory(0, bytesWritten);
            //long blockId = block.Number;

            //await _eventSender.SendAsync("BlockAdded", block);

            //using (WritingBlockDuration.NewTimer())
            //{
            //if (!_sessionPool.TryGet(out var session))
            //    session = await _sessionPool.GetAsync().ConfigureAwait(false);

            await _session.UpsertAsync(key, value);
            //while (result.Status.IsPending)
            //    result = await result.CompleteAsync();
            //_sessionPool.Return(session);
            //return result.Status;
            //}

            // Flushes partially filled batches, does not wait for response
            _session.Flush();
        }

        public async Task<(bool, Block)> TryGetBlockById(uint blockNum)
        {
            int blockNumLength = _encode.GetByteCount(blockNum.ToString());

            byte[] blockNumBytes = _pool.Rent(blockNumLength);
            int bytesWritten = _encode.GetBytes(blockNum.ToString(), blockNumBytes);
            var key = blockNumBytes.AsMemory(0, bytesWritten);
            //using (BlockReaderSessionReadDurationSummary.NewTimer())
            //{
            //if (!_sessionPool.TryGet(out var session))
            //    session = await _sessionPool.GetAsync().ConfigureAwait(false);
            var (status, output) = await _session.ReadAsync(key);
            //_sessionPool.Return(session);

            var block = JsonSerializer.Deserialize<Block>(output.Item1.Memory.Span.Slice(0, output.Item2));
            return (status.Found, block);
            //}
        }

    }
}