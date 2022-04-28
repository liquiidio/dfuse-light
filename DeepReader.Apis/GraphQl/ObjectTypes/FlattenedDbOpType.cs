using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class FlattenedDbOpType : ObjectType<FlattenedDbOp>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedDbOp> descriptor)
        {
            descriptor.Name("FlattenedDbOp"); // Todo (Haron) FlattenedDbOp or DbOp GraphQL
            descriptor.Field(f => f.Operation).Type<StringType>().Name("operation");
            descriptor.Field(f => f.Code).Type<CustomScalarTypes.NameType>().Name("code");
            descriptor.Field(f => f.Scope).Type<CustomScalarTypes.NameType>().Name("scope");
            descriptor.Field(f => f.TableName).Type<CustomScalarTypes.NameType>().Name("tableName");
            descriptor.Field(f => f.PrimaryKey).Type<ByteArrayType>().Name("primaryKey");
            descriptor.Field(f => f.OldPayer).Type<CustomScalarTypes.NameType>().Name("oldPayer");
            descriptor.Field(f => f.NewPayer).Type<CustomScalarTypes.NameType>().Name("newPayer");
            descriptor.Field(f => f.OldData).Type<ByteArrayType>().Name("oldData");
            descriptor.Field(f => f.NewData).Type<ByteArrayType>().Name("newData");
        }
    }
}