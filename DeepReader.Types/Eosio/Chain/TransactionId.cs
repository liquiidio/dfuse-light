using System.Text.Json.Serialization;
using DeepReader.Types.Helpers;
using DeepReader.Types.JsonConverters;
using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// Custom type due to Variant-Handling
/// </summary>
[JsonConverter(typeof(TransactionIdJsonConverter))]
public sealed class TransactionId : TransactionVariant
{
    // can't inherit directly from PooledObject here
    // ReSharper disable once InconsistentNaming
    private static readonly ObjectPool<TransactionId> TypeObjectPool = new DefaultObjectPool<TransactionId>(new DefaultPooledObjectPolicy<TransactionId>());

    public static TransactionId FromPool()
    {
        return TypeObjectPool.Get();
    }

    public static void ReturnToPool(TransactionId obj)
    {
        TypeObjectPool.Return(obj);
    }

    // end can't inherit directly from PooledObject here

    private const int Checksum256ByteLength = 32;
    
    [JsonIgnore]
    public byte[] Binary = Array.Empty<byte>();

    private string? _stringVal;
    public string StringVal
    {
        get => _stringVal ??= SerializationHelper.ByteArrayToHexString(Binary);
        set => _stringVal = value;
    }

    public TransactionId()
    {

    }

    public TransactionId(string transactionId)
    {
        Binary = SerializationHelper.StringToByteArray(transactionId);
        _stringVal = transactionId;
    }

    public new static TransactionId ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = TypeObjectPool.Get();

        obj.Binary = reader.ReadBytes(Checksum256ByteLength);

        return obj;
    }

    public new static TransactionId ReadFromBinaryReaderWithoutPooling(BinaryReader reader)
    {
        var obj = new TransactionId
        {
            Binary = reader.ReadBytes(Checksum256ByteLength)
        };

        return obj;
    }

    public new void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Binary);
        _stringVal = null;

        ReturnToPool(this);
    }

    public static implicit operator TransactionId(string value)
    {
        return new TransactionId(){ StringVal = value };    // TODO string to Binary
    }

    public static implicit operator TransactionId(ReadOnlySpan<char> value)
    {
        return new TransactionId() { StringVal = value.ToString() };    // TODO string to Binary or (even better) span to binary
    }

    public static implicit operator string(TransactionId value)
    {
        return value.StringVal;
    }

    public static implicit operator TransactionId(byte[] binary)
    {
        return new TransactionId(){ Binary = binary };
    }

    public static implicit operator byte[](TransactionId value)
    {
        return value.Binary;
    }

    public override string ToString()
    {
        return StringVal;
    }

    public static readonly TransactionId TypeEmpty = new();
}