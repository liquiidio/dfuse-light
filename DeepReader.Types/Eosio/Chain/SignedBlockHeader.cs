using DeepReader.Types.Helpers;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class SignedBlockHeader : BlockHeader, IEosioSerializable<SignedBlockHeader>
{
    [SortOrder(10)]
    public Signature ProducerSignature = Signature.Empty;// ecc.Signature // no pointer!!

    public new static SignedBlockHeader ReadFromBinaryReader(BinaryReader reader)
    {
        var signedBlockHeader = new SignedBlockHeader()
        {
            Timestamp = reader.ReadTimestamp(),
            Producer = reader.ReadName(),
            Confirmed = reader.ReadUInt16(),
            Previous = reader.ReadChecksum256(),
            TransactionMroot = reader.ReadChecksum256(),
            ActionMroot = reader.ReadChecksum256(),
            ScheduleVersion = reader.ReadUInt32(),
        };

        var readProducer = reader.ReadBoolean();

        if (readProducer)
            signedBlockHeader.NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);

        signedBlockHeader.HeaderExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedBlockHeader.HeaderExtensions.Length; i++)
        {
            signedBlockHeader.HeaderExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }
        signedBlockHeader.ProducerSignature = reader.ReadSignature();
        return signedBlockHeader;
    }
}