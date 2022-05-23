using DeepReader.Classes;
using DeepReader.Types.Interfaces;
using KGySoft.CoreLibraries;
using Microsoft.IO;
using Prometheus;
using System.Buffers;

namespace DeepReader.DeepMindDeserializer;

public static class DeepMindDeserializer
{
    // This should keep a count of the blocks created, that can be used to determine blocks per second (theoretically)
    private static readonly Counter DeserializedBlocksCount = Metrics.CreateCounter("deepreader_deserialized_blocks_count", "Number of deserialized blocks");

    private static readonly RecyclableMemoryStreamManager StreamManager = new();
    private static ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;


    public static async Task<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken) where T : IEosioSerializable<T>
    {
        return await Task.Run(() => Deserialize<T>(data), cancellationToken);
    }

    public static T Deserialize<T>(StringSegment chunk) where T : IEosioSerializable<T>
    {
        DeserializedBlocksCount.Inc(); // TODO @Haron this seems not to be correct as it increments not only for Blocks but also for any other Type

        var length = chunk.Length >> 1;
        var bytes = ArrayPool.Rent(length);// rent bytes from pool
        Decoder.HexToBytes(chunk, bytes, length);
        using var stream = StreamManager.GetStream(bytes);// copies buffer/bytes
        using var reader = new BinaryReader(stream);
        ArrayPool<byte>.Shared.Return(bytes);// return buffer/bytes

        return T.ReadFromBinaryReader(reader);
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> bytes) where T : IEosioSerializable<T>
    {
        DeserializedBlocksCount.Inc();// TODO @Haron this seems not to be correct as it increments not only for Blocks but also for any other Type

        using var stream = StreamManager.GetStream(bytes);
        using var reader = new BinaryReader(stream);
        var obj = T.ReadFromBinaryReader(reader);

        return obj;
    }
}