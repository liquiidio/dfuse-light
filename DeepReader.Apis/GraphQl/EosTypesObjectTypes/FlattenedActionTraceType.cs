using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class FlattenedActionTraceType : ObjectType<FlattenedActionTrace>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedActionTrace> descriptor)
        {
            descriptor.Field(f => f.Receiver).Type<NameType>().Name("Receiver");
            descriptor.Field(f => f.Act).Type<ActionType>().Name("Act");
            descriptor.Field(f => f.ContextFree).Type<BooleanType>().Name("ContextFree");
            descriptor.Field(f => f.ElapsedUs).Type<LongType>().Name("ElapsedUs");
            descriptor.Field(f => f.Console).Type<StringType>().Name("Console");
            descriptor.Field(f => f.AccountRamDeltas).Type<ListType<AccountDeltaType>>().Name("AccountRamDeltas");
            descriptor.Field(f => f.RamOps).Type<ListType<FlattenedRamOpType>>().Name("RamOps");
            descriptor.Field(f => f.DbOps).Type<ListType<FlattenedDbOpType>>().Name("DbOps");
            descriptor.Field(f => f.TableOps).Type<ListType<FlattenedTableOpType>>().Name("TableOps");
            descriptor.Field(f => f.ReturnValue).Type<StringType>().Name("ReturnValue"); 
            // Todo (Haron), string? 
        }
    }
}