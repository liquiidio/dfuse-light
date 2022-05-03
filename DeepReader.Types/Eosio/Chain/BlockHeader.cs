using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class BlockHeader : IEosioSerializable<BlockHeader>
{
    [SortOrder(1)]
    public Timestamp Timestamp;
    [SortOrder(2)]
    public Name Producer;
    [SortOrder(3)]
    public ushort Confirmed;
    [SortOrder(4)]
    public Checksum256 Previous;
    [SortOrder(5)]
    public Checksum256 TransactionMroot;
    [SortOrder(6)]
    public Checksum256 ActionMroot;
    [SortOrder(7)]
    public uint ScheduleVersion;
    [SortOrder(8)]
    public ProducerSchedule? NewProducers;
    [SortOrder(9)]
    public Extension[] HeaderExtensions;

    public BlockHeader(BinaryReader reader)
    {
        Timestamp = reader.ReadTimestamp();
        Producer = reader.ReadName();
        Confirmed = reader.ReadUInt16();
        Previous = reader.ReadChecksum256();
        TransactionMroot = reader.ReadChecksum256();
        ActionMroot = reader.ReadChecksum256();
        ScheduleVersion = reader.ReadUInt32();

        var readProducer = reader.ReadBoolean();

        if (readProducer)
            NewProducers = ProducerSchedule.ReadFromBinaryReader(reader);

        HeaderExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < HeaderExtensions.Length; i++)
        {
            HeaderExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.Read7BitEncodedInt()));
        }
    }

    public static BlockHeader ReadFromBinaryReader(BinaryReader reader)
    {
        return new BlockHeader(reader);
    }
}