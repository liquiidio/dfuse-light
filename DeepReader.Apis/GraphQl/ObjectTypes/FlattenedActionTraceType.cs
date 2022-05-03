using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class FlattenedActionTraceType : ObjectType<FlattenedActionTrace>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedActionTrace> descriptor)
        {
            descriptor.Name("ActionTrace");
            descriptor.Field(f => f.Receiver).Type<CustomScalarTypes.NameType>().Name("receiver");
            descriptor.Field(f => f.Act).Type<ActionType>().Name("act");
            descriptor.Field(f => f.ContextFree).Type<BooleanType>().Name("contextFree");
            descriptor.Field(f => f.ElapsedUs).Type<LongType>().Name("elapsedUs");
            descriptor.Field(f => f.Console).Type<StringType>().Name("console");
            descriptor.Field(f => f.AccountRamDeltas).Type<ListType<AccountDeltaType>>().Name("accountRamDeltas");
            descriptor.Field(f => f.RamOps).Type<ListType<FlattenedRamOpType>>().Name("ramOps");
            descriptor.Field(f => f.DbOps).Type<ListType<FlattenedDbOpType>>().Name("dbOps");
            descriptor.Field(f => f.TableOps).Type<ListType<FlattenedTableOpType>>().Name("tableOps");
            descriptor.Field(f => f.ReturnValue).Type<StringType>().Name("returnValue"); 
            // Todo (Haron), string? 
        }
    }
}