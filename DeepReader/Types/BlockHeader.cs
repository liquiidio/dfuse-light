namespace DeepReader.Types;

public class BlockHeader
{
    public Timestamp Timestamp;//        *timestamp.Timestamp `protobuf:"bytes,3,opt,name=timestamp,proto3" json:"timestamp,omitempty"`
    public string Producer;//         string               `protobuf:"bytes,4,opt,name=producer,proto3" json:"producer,omitempty"`
    public uint Confirmed;//        uint32               `protobuf:"varint,5,opt,name=confirmed,proto3" json:"confirmed,omitempty"`
    public string Previous;//         string               `protobuf:"bytes,6,opt,name=previous,proto3" json:"previous,omitempty"`
    public byte[] TransactionMroot;// []byte               `protobuf:"bytes,7,opt,name=transaction_mroot,json=transactionMroot,proto3" json:"transaction_mroot,omitempty"`
    public byte[] ActionMroot;//      []byte               `protobuf:"bytes,8,opt,name=action_mroot,json=actionMroot,proto3" json:"action_mroot,omitempty"`
    public uint ScheduleVersion;//  uint32               `protobuf:"varint,9,opt,name=schedule_version,json=scheduleVersion,proto3" json:"schedule_version,omitempty"`
    public Extension[] HeaderExtensions;// []*Extension         `protobuf:"bytes,11,rep,name=header_extensions,json=headerExtensions,proto3" json:"header_extensions,omitempty"`
    // EOSIO 1.x only
    //
    // A change to producer schedule was reported as a `NewProducers` field on the
    // `BlockHeader` in EOSIO 1.x. In EOSIO 2.x, when feature `WTMSIG_BLOCK_SIGNATURES`
    // is activated, the `NewProducers` field is not present anymore and the schedule change
    // is reported through a `BlockHeaderExtension` on the the `BlockHeader` struct.
    //
    // If you need to access the old value, you can
    public ProducerSchedule NewProducersV1;//       *ProducerSchedule `protobuf:"bytes,10,opt,name=new_producers_v1,json=newProducersV1,proto3" json:"new_producers_v1,omitempty"`
    //XXX_NoUnkeyedLiteral struct{}          `json:"-"`
    //XXX_unrecognized     []byte            `json:"-"`
    //XXX_sizecache        int32             `json:"-"`
}