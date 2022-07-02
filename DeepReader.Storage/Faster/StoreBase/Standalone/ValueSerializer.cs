using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;
using FASTER.core;

namespace DeepReader.Storage.Faster.Base.Standalone
{
    internal class ValueSerializer<TValue> : BinaryObjectSerializer<TValue>
        where TValue : IFasterSerializable<TValue>
    {
        public override void Deserialize(out TValue obj)
        {
            obj = TValue.ReadFromFaster((IBufferReader)reader);
        }

        public override void Serialize(ref TValue obj)
        {
            obj.WriteToFaster((IBufferWriter)writer);
        }
    }
}
