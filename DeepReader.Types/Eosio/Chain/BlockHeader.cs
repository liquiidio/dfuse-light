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
}