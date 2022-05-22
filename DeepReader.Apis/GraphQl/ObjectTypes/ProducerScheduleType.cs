using DeepReader.Apis.GraphQl.CustomScalarTypes;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Apis.GraphQl.ObjectTypes
{
    internal class ProducerScheduleType : ObjectType<ProducerSchedule>
    {
        protected override void Configure(IObjectTypeDescriptor<ProducerSchedule> descriptor)
        {
            descriptor.Name("ProducerSchedule");
            descriptor.Field(f => f.Producers).Type<ListType<ProducerKeyType>>().Name("producers");
            descriptor.Field(f => f.Version).Type<UnsignedIntType>().Name("version");
        }
    }
}