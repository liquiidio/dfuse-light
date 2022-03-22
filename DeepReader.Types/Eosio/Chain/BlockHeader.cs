using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class BlockHeader : IEosioSerializable<BlockHeader>
{
    [SortOrder(1)]
    public Timestamp Timestamp = Timestamp.Zero;
    [SortOrder(2)]
    public Name Producer = Name.Empty;
    [SortOrder(3)]
    public ushort Confirmed = 0;
    [SortOrder(4)]
    public Checksum256 Previous = Checksum256.Empty;
    [SortOrder(5)]
    public Checksum256 TransactionMroot = Checksum256.Empty;
    [SortOrder(6)]
    public Checksum256 ActionMroot = Checksum256.Empty;
    [SortOrder(7)]
    public uint ScheduleVersion = 0;
    [SortOrder(8)]
    public ProducerSchedule? NewProducers;
    [SortOrder(9)]
    public Extension[] HeaderExtensions = Array.Empty<Extension>();

    public static BlockHeader ReadFromBinaryReader(BinaryReader reader)
    {
        var blockHeader = new BlockHeader()
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
            blockHeader.NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);


        blockHeader.HeaderExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < blockHeader.HeaderExtensions.Length; i++)
        {
            blockHeader.HeaderExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }

        return blockHeader;
    }
}