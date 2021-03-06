using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public sealed class PackedTransaction : TransactionVariant, IEosioSerializable<PackedTransaction>
{
    // can't inherit directly from PooledObject here
    // ReSharper disable once InconsistentNaming
    private static readonly ObjectPool<PackedTransaction> TypeObjectPool = new DefaultObjectPool<PackedTransaction>(new DefaultPooledObjectPolicy<PackedTransaction>());

    public static PackedTransaction FromPool()
    {
        return TypeObjectPool.Get();
    }

    public static void ReturnToPool(PackedTransaction obj)
    {
        TypeObjectPool.Return(obj);
    }

    // end can't inherit directly from PooledObject here
    public Signature[] Signatures = Array.Empty<Signature>();
    // TODO @corvin Compression to enum
    public byte Compression; //fc::enum_type<uint8_t, compression>
    public Bytes PackedContextFreeData = new();
    public Bytes PackedTrx = new ();

    public PackedTransaction()
    {
        
    }

    public new static PackedTransaction ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        var obj = TypeObjectPool.Get();

        obj.Signatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Signatures.Length; i++)
        {
            obj.Signatures[i] = Signature.ReadFromBinaryReader(reader);
        }

        obj.Compression = reader.ReadByte();

        obj.PackedContextFreeData = reader.ReadBytes(reader.Read7BitEncodedInt());
        obj.PackedTrx = reader.ReadBytes(reader.Read7BitEncodedInt());

        return obj;
    }

    public new static PackedTransaction ReadFromBinaryReaderWithoutPooling(BinaryReader reader)
    {
        var obj = new PackedTransaction();

        obj.Signatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Signatures.Length; i++)
        {
            obj.Signatures[i] = Signature.ReadFromBinaryReader(reader);
        }

        obj.Compression = reader.ReadByte();

        obj.PackedContextFreeData = reader.ReadBytes(reader.Read7BitEncodedInt());
        obj.PackedTrx = reader.ReadBytes(reader.Read7BitEncodedInt());

        return obj;
    }

    public new void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Signatures.Length);
        foreach (var signature in Signatures)
        {
            signature.WriteToBinaryWriter(writer);
        }

        writer.Write(Compression);

        writer.Write(PackedContextFreeData.Binary.Length);
        writer.Write(PackedContextFreeData.Binary);

        writer.Write(PackedTrx.Binary.Length);
        writer.Write(PackedTrx.Binary);
    }
}