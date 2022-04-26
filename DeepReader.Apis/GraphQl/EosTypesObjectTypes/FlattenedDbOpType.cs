using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class FlattenedDbOpType : ObjectType<FlattenedDbOp>
    {
        protected override void Configure(IObjectTypeDescriptor<FlattenedDbOp> descriptor)
        {
            descriptor.Field(f => f.Operation).Type<EnumType>().Name("Operation");
            descriptor.Field(f => f.Code).Type<NameType>().Name("Code");
            descriptor.Field(f => f.Scope).Type<NameType>().Name("Scope");
            descriptor.Field(f => f.TableName).Type<NameType>().Name("TableName");
            descriptor.Field(f => f.PrimaryKey).Type<ByteArrayType>().Name("PrimaryKey");
            descriptor.Field(f => f.OldPayer).Type<NameType>().Name("OldPayer");
            descriptor.Field(f => f.NewPayer).Type<NameType>().Name("NewPayer");
            descriptor.Field(f => f.OldData).Type<ByteArrayType>().Name("OldData");
            descriptor.Field(f => f.NewData).Type<ByteArrayType>().Name("NewData");
        }
    }
}