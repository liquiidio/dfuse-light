using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/transaction.hpp
/// </summary>
public class PackedTransaction : TransactionVariant, IEosioSerializable<PackedTransaction>
{
    public Signature[] Signatures = Array.Empty<Signature>();
    // TODO @corvin Compression to enum
    public byte Compression = 0; //fc::enum_type<uint8_t, compression>
    public Bytes PackedContextFreeData = new();
    public Bytes PackedTrx = new ();

    public PackedTransaction(BinaryReader reader)
    {
        Signatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < Signatures.Length; i++)
        {
            Signatures[i] = reader.ReadSignature();
        }

        Compression = reader.ReadByte();

        PackedContextFreeData = reader.ReadBytes(reader.Read7BitEncodedInt());
        PackedTrx = reader.ReadBytes(reader.Read7BitEncodedInt());
    }

    public new static PackedTransaction ReadFromBinaryReader(BinaryReader reader)
    {
        return new PackedTransaction(reader);
    }

    public new void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Signatures.Length);
        foreach (var signature in Signatures)
        {
            writer.WriteSignature(signature);
        }

        writer.Write(Compression);

        writer.Write(PackedContextFreeData.Binary.Length);
        writer.Write(PackedContextFreeData.Binary);

        writer.Write(PackedTrx.Binary.Length);
        writer.Write(PackedTrx.Binary);
    }
}