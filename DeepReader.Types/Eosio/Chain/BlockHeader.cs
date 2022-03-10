using DeepReader.Types.Helpers;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_header.hpp
/// </summary>
public class BlockHeader
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

    // Todo: @corvin start from here
    public static BlockHeader ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new BlockHeader()
        {
            Timestamp = reader.ReadUInt32(),
            Producer = reader.ReadUInt64(),
            Confirmed = reader.ReadUInt16(),
            Previous = reader.ReadBytes(32),
            TransactionMroot = reader.ReadBytes(32),
            ActionMroot = reader.ReadBytes(32),
            ScheduleVersion = reader.ReadUInt32(),
            NewProducers = ProducerSchedule.ReadFromBinaryReader(reader)
        };

        obj.HeaderExtensions = new Extension[reader.Read7BitEncodedInt()];
        for (int i = 0; i < obj.HeaderExtensions.Length; i++)
        {
            obj.HeaderExtensions[i] = new Extension(reader.ReadUInt16(), reader.ReadChars(reader.ReadInt32()));
        }

        return obj;
    }
}