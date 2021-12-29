using DeepReader.EosTypes;

namespace DeepReader.Types;

public class BlockHeader
{
    [SortOrder(1)]
    public Timestamp Timestamp = 0;//*timestamp.Timestamp
    [SortOrder(2)]
    public Name Producer = string.Empty;//string
    [SortOrder(3)] 
    public ushort Confirmed = 0;//uint32
    [SortOrder(4)] 
    public Checksum256 Previous = string.Empty;//string
    [SortOrder(5)] 
    public Checksum256 TransactionMroot = string.Empty;//[]byte
    [SortOrder(6)] 
    public Checksum256 ActionMroot = string.Empty;//[]byte
    [SortOrder(7)] 
    public uint ScheduleVersion = 0;//uint32
    // EOSIO 1.x only
    //
    // A change to producer schedule was reported as a `NewProducers` field on the
    // `BlockHeader` in EOSIO 1.x. In EOSIO 2.x, when feature `WTMSIG_BLOCK_SIGNATURES`
    // is activated, the `NewProducers` field is not present anymore and the schedule change
    // is reported through a `BlockHeaderExtension` on the the `BlockHeader` struct.
    //
    // If you need to access the old value, you can
    [SortOrder(8)] 
    public ProducerSchedule? NewProducers;//*ProducerSchedule // TODO
    [SortOrder(9)]
    public Extension[] HeaderExtensions = Array.Empty<Extension>();//[]*Extension
}