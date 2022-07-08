using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using FASTER.core;

namespace DeepReader.Storage.Faster.StoreBase.Standalone
{
    internal class ValueSerializer<TValue> : BinaryObjectSerializer<TValue>
        where TValue : IFasterSerializable<TValue>
    {
        public override void Deserialize(out TValue obj)
        {
            obj = TValue.ReadFromFaster(new Types.Infrastructure.BinaryReaders.BinaryReader(reader));
        }

        public override void Serialize(ref TValue obj)
        {
            obj.WriteToFaster(new Types.Infrastructure.BinaryWriters.BinaryWriter(writer));
        }
    }
}
