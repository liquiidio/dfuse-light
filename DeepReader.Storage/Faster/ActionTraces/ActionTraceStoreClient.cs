using DeepReader.Types.StorageTypes;
using FASTER.client;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DeepReader.Storage.Faster.ActionTraces
{
    internal class ActionTraceStoreClient
    {
        private const string ip = "127.0.0.1";
        private const int port = 5005;
        private static Encoding _encode = Encoding.UTF8;
        private static ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

        private readonly FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> _client;

        private ClientSession<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, (IMemoryOwner<byte>, int), byte, ActionTraceMemoryFunctions, MemoryParameterSerializer<byte>> _session;

        public ActionTraceStoreClient()
        {
            _client = new FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(ip, port);
            _session = _client.NewSession(new ActionTraceMemoryFunctions());
        }

        public async Task WriteActionTrace(ActionTrace actionTrace)
        {
            Debug.WriteLine("Writing Action Trace");
            int actionTraceIdLength = _encode.GetByteCount(actionTrace.GlobalSequence.ToString());
            int actionTraceLength = _encode.GetByteCount(JsonSerializer.Serialize(actionTrace));

            byte[] actionTraceIdBytes = _pool.Rent(actionTraceIdLength);
            int bytesWritten = _encode.GetBytes(actionTrace.GlobalSequence.ToString(), actionTraceIdBytes);
            var key = actionTraceIdBytes.AsMemory(0, bytesWritten);

            byte[] actionTraceBytes = _pool.Rent(actionTraceLength);
            bytesWritten = _encode.GetBytes(JsonSerializer.Serialize(actionTrace), actionTraceBytes);
            var value = actionTraceBytes.AsMemory(0, bytesWritten);

            await _session.UpsertAsync(key, value);

            // Flushes partially filled batches, does not wait for response
            _session.Flush();
        }

        public async Task<(bool, ActionTrace)> TryGetActionTraceById(ulong globalSequence)
        {
            int globalSequenceLength = _encode.GetByteCount(globalSequence.ToString());

            byte[] blockNumBytes = _pool.Rent(globalSequenceLength);
            int bytesWritten = _encode.GetBytes(globalSequence.ToString(), blockNumBytes);
            var key = blockNumBytes.AsMemory(0, bytesWritten);
            
            var (status, output) = await _session.ReadAsync(key);

            var actionTrace = JsonSerializer.Deserialize<ActionTrace>(output.Item1.Memory.Span.Slice(0, output.Item2));
            return (status.Found, actionTrace);
        }

    }
}