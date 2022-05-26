using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class BlockHeader : IEosioSerializable<BlockHeader>
{
    public Timestamp Timestamp;

    public Name Producer;

    public ushort Confirmed;

    public Checksum256 Previous;

    public Checksum256 TransactionMroot;

    public Checksum256 ActionMroot;

    public uint ScheduleVersion;

    public ProducerSchedule? NewProducers;

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