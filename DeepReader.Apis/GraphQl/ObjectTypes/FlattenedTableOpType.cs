using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class FlattenedTableOpType : ObjectType<FlattenedTableOp>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedTableOp> descriptor)
        {
            descriptor.Name("FlattenedTableOp");
            descriptor.Field(f => f.Operation).Type<StringType>().Name("operation");
            descriptor.Field(f => f.Payer).Type<CustomScalarTypes.NameType>().Name("payer");
            descriptor.Field(f => f.Code).Type<CustomScalarTypes.NameType>().Name("code");
            descriptor.Field(f => f.Scope).Type<CustomScalarTypes.NameType>().Name("scope");
            descriptor.Field(f => f.TableName).Type<CustomScalarTypes.NameType>().Name("tableName");
        }
    }
}