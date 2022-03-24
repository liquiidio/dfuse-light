using DeepReader.Types.Helpers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block.hpp
/// </summary>
public class SignedBlock : SignedBlockHeader, IEosioSerializable<SignedBlock>
{
    [SortOrder(11)]
    public TransactionReceipt[] Transactions = Array.Empty<TransactionReceipt>();
    [SortOrder(12)]
    public Extension[] BlockExtensions = Array.Empty<Extension>();

    public new static SignedBlock ReadFromBinaryReader(BinaryReader reader)
    {
        var signedBlock = new SignedBlock()
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
            signedBlock.NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);

        signedBlock.HeaderExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedBlock.HeaderExtensions.Length; i++)
        {
            signedBlock.HeaderExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }
        signedBlock.ProducerSignature = reader.ReadSignature();

        signedBlock.Transactions = new TransactionReceipt[reader.Read7BitEncodedInt()];
        for (int i = 0; i < signedBlock.Transactions.Length; i++)
        {
            signedBlock.Transactions[i] = TransactionReceipt.ReadFromBinaryReader(reader);
        }

        signedBlock.BlockExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i != signedBlock.BlockExtensions.Length; i++)
        {
            signedBlock.BlockExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }

        return signedBlock;
    }
}