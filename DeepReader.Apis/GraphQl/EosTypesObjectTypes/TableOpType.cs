using DeepReader.Types;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class TableOpType : ObjectType<TableOp>
    {
        protected override void Configure(IObjectTypeDescriptor<TableOp> descriptor)
        {
            descriptor.Field(f => f.Operation).Type<EnumType>().Name("Operation");
            descriptor.Field(f => f.ActionIndex).Type<LongType>().Name("ActionIndex");
            descriptor.Field(f => f.Payer).Type<NameType>().Name("Payer");
            descriptor.Field(f => f.Code).Type<NameType>().Name("Code");
            descriptor.Field(f => f.Scope).Type<NameType>().Name("Scope");
            descriptor.Field(f => f.TableName).Type<NameType>().Name("TableName");
        }
    }
}