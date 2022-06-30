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

    public BlockHeader(IBufferReader reader)
    {
        Timestamp = Timestamp.ReadFromBinaryReader(reader);
        Producer = Name.ReadFromBinaryReader(reader);
        Confirmed = reader.ReadUInt16();
        Previous = Checksum256.ReadFromBinaryReader(reader);
        TransactionMroot = Checksum256.ReadFromBinaryReader(reader);
        ActionMroot = Checksum256.ReadFromBinaryReader(reader);
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

    public static BlockHeader ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new BlockHeader(reader);
    }
}