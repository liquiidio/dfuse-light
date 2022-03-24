using DeepReader.Types.EosTypes;
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

    public new static PackedTransaction ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new PackedTransaction();

        obj.Signatures = new Signature[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.Signatures.Length; i++)
        {
            obj.Signatures[i] = reader.ReadSignature();
        }

        obj.Compression = reader.ReadByte();

        obj.PackedContextFreeData = reader.ReadBytes(reader.Read7BitEncodedInt());
        obj.PackedTrx = reader.ReadBytes(reader.Read7BitEncodedInt());

        return obj;
    }
}