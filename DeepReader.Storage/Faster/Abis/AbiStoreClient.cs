using DeepReader.Types.EosTypes;
using FASTER.client;
using System.Buffers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace DeepReader.Storage.Faster.Abis
{
    internal class AbiStoreClient
    {
        private const string ip = "127.0.0.1";
        private const int port = 5004;
        private static Encoding _encode = Encoding.UTF8;
        private static ArrayPool<byte> _pool = ArrayPool<byte>.Shared;

        private readonly FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>> _client;

        private ClientSession<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, (IMemoryOwner<byte>, int), byte, AbiMemoryFunctions, MemoryParameterSerializer<byte>> _session;

        public AbiStoreClient()
        {
            _client = new FasterKVClient<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>(ip, port);
            _session = _client.NewSession(new AbiMemoryFunctions());
        }

        public async Task WriteAbi(AbiCacheItem abi)
        {
            int abiIdLength = _encode.GetByteCount(abi.Id.ToString());
            int abiLength = _encode.GetByteCount(JsonSerializer.Serialize(abi));

            byte[] abiIdBytes = _pool.Rent(abiIdLength);
            int bytesWritten = _encode.GetBytes(abi.Id.ToString(), abiIdBytes);
            var key = abiIdBytes.AsMemory(0, bytesWritten);

            byte[] abiBytes = _pool.Rent(abiLength);
            bytesWritten = _encode.GetBytes(JsonSerializer.Serialize(abi), abiBytes);
            var value = abiBytes.AsMemory(0, bytesWritten);

            await _session.UpsertAsync(key, value);

            // Flushes partially filled batches, does not wait for response
            _session.Flush();
        }

        public async Task UpsertAbi(Name account, ulong globalSequence, Assembly assembly)
        {
            //int abiIdLength = _encode.GetByteCount(account.IntVal.ToString());
            //int abiLength = _encode.GetByteCount(JsonSerializer.Serialize(abi));

            //byte[] abiIdBytes = _pool.Rent(abiIdLength);
            //int bytesWritten = _encode.GetBytes(abi.Id.ToString(), abiIdBytes);
            //var key = abiIdBytes.AsMemory(0, bytesWritten);

            //await _session.RMWAsync(account.IntVal, new AbiInput(account.IntVal, globalSequence, assembly));
        }

        public async Task<(bool, AbiCacheItem)> TryGetAbiAssembliesById(Name account)
        {
            int abiIdLength = _encode.GetByteCount(account.IntVal.ToString());

            byte[] abiIdBytes = _pool.Rent(abiIdLength);
            int bytesWritten = _encode.GetBytes(account.IntVal.ToString(), abiIdBytes);
            var key = abiIdBytes.AsMemory(0, bytesWritten);

            var (status, output) = await _session.ReadAsync(key);

            var abi = JsonSerializer.Deserialize<AbiCacheItem>(output.Item1.Memory.Span.Slice(0, output.Item2));
            return (status.Found, abi);
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetAbiAssemblyByIdAndGlobalSequence(Name account, ulong globalSequence)
        {
            int abiIdLength = _encode.GetByteCount(account.IntVal.ToString());

            byte[] abiIdBytes = _pool.Rent(abiIdLength);
            int bytesWritten = _encode.GetBytes(account.IntVal.ToString(), abiIdBytes);
            var key = abiIdBytes.AsMemory(0, bytesWritten);

            var (status, output) = await _session.ReadAsync(key);
            var abi = JsonSerializer.Deserialize<AbiCacheItem>(output.Item1.Memory.Span.Slice(0, output.Item2));


            if (status.Found && abi.AbiVersions.Any(av => av.Key <= globalSequence))
            {
                // returns the index of the Abi matching the globalSequence or binary complement of the next item (negative)
                var abiVersionIndex = abi.AbiVersions.Keys.ToList().BinarySearch(globalSequence);

                // if negative, revert the binary complement
                if (abiVersionIndex < 0)
                    abiVersionIndex = ~abiVersionIndex;
                // we always want the previous Abi-version
                if (abiVersionIndex > 0)
                    abiVersionIndex--;

                var abiVersionsArry = abi.AbiVersions.ToArray();
                if (abiVersionIndex >= 0 && abiVersionsArry.Length > abiVersionIndex)
                    return (status.Found, abiVersionsArry[abiVersionIndex]);
            }
            return (false, new KeyValuePair<ulong, AssemblyWrapper>());
        }

        public async Task<(bool, KeyValuePair<ulong, AssemblyWrapper>)> TryGetActiveAbiAssembly(Name account)
        {
            int abiIdLength = _encode.GetByteCount(account.IntVal.ToString());

            byte[] abiIdBytes = _pool.Rent(abiIdLength);
            int bytesWritten = _encode.GetBytes(account.IntVal.ToString(), abiIdBytes);
            var key = abiIdBytes.AsMemory(0, bytesWritten);

            var (status, output) = await _session.ReadAsync(key);
            var abi = JsonSerializer.Deserialize<AbiCacheItem>(output.Item1.Memory.Span.Slice(0, output.Item2));

            if (status.Found && abi.AbiVersions.Count > 0)
            {
                return (status.Found, abi.AbiVersions.Last());
            }
            return (false, new KeyValuePair<ulong, AssemblyWrapper>());

        }
    }
}