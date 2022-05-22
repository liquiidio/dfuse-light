using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class ActionTraceType : ObjectType<ActionTrace>
    {
        protected override void Configure(IObjectTypeDescriptor<ActionTrace> descriptor)
        {
            descriptor.Name("ActionTrace");
            descriptor.Field(f => f.GlobalSequence).Type<UnsignedLongType>().Name("seq");
            descriptor.Field(f => f.Receipt).Type<ActionReceiptType>().Name("receipt");
            descriptor.Field(f => f.Receiver).Type<CustomScalarTypes.NameType>().Name("receiver");
            descriptor.Field(f => f.Act.Account).Type<CustomScalarTypes.NameType>().Name("account");
            descriptor.Field(f => f.Act.Name).Type<CustomScalarTypes.NameType>().Name("name");
            descriptor.Field(f => f.Act.Authorization).Type<ListType<PermissionLevelType>>().Name("authorization");
            descriptor.Field(f => f.Act.Data).Type<ByteArrayType>().Name("data");
            descriptor.Field(f => f.Act.Data.Json).Type<HotChocolate.Types.StringType>().Name("json"); // TODO
            descriptor.Field(f => f.Act.Data.Hex).Type<HotChocolate.Types.StringType>().Name("hexData"); // TODO
            descriptor.Field(f => f.RamOps).Type<ListType<RamOpType>>().Name("ramOps");
            descriptor.Field(f => f.DbOps).Type<ListType<DbOpType>>().Name("dbOps");
            descriptor.Field(f => f.TableOps).Type<ListType<TableOpType>>().Name("tableOps");
            descriptor.Field(f => f.Console).Type<StringType>().Name("console");
            descriptor.Field(f => f.ContextFree).Type<BooleanType>().Name("contextFree");
            descriptor.Field(f => f.ElapsedUs).Type<LongType>().Name("elapsed");
            descriptor.Field(f => f.IsNotify).Type<BooleanType>().Name("isNotify");
            descriptor.Field(f => f.ReturnValue).Type<StringType>().Name("returnValue");
            descriptor.Field(f => f.CreatedActions).Type<ListType<ActionTraceType>>().Name("createdActions");
            descriptor.Field(f => f.CreatorAction).Type<ActionTraceType>().Name("creatorAction");

            // Todo (Haron), string? 
        }
    }
}