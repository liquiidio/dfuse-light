using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class FlattenedTableOpType : ObjectType<FlattenedTableOp>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedTableOp> descriptor)
        {
            descriptor.Field(f => f.Operation).Type<StringType>().Name("Operation");
            descriptor.Field(f => f.Payer).Type<NameType>().Name("Payer");
            descriptor.Field(f => f.Code).Type<NameType>().Name("Code");
            descriptor.Field(f => f.Scope).Type<NameType>().Name("Scope");
            descriptor.Field(f => f.TableName).Type<NameType>().Name("TableName");
        }
    }
}