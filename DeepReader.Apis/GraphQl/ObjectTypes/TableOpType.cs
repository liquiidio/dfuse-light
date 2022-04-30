using DeepReader.Types;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class TableOpType : ObjectType<TableOp>
    {
        protected override void Configure(IObjectTypeDescriptor<TableOp> descriptor)
        {
            descriptor.Name("TableOp");
            descriptor.Field(f => f.Operation).Type<StringType>().Name("operation");
            descriptor.Field(f => f.ActionIndex).Type<UnsignedIntType>().Name("actionIndex");
            descriptor.Field(f => f.Payer).Type<CustomScalarTypes.NameType>().Name("payer");
            descriptor.Field(f => f.Code).Type<CustomScalarTypes.NameType>().Name("code");
            descriptor.Field(f => f.Scope).Type<CustomScalarTypes.NameType>().Name("scope");
            descriptor.Field(f => f.TableName).Type<CustomScalarTypes.NameType>().Name("tableName");
        }
    }
}