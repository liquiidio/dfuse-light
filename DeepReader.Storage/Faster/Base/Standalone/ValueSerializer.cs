using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Interfaces;
using FASTER.core;

namespace DeepReader.Storage.Faster.Base.Standalone
{
    internal class ValueSerializer<TValue> : BinaryObjectSerializer<TValue>
        where TValue : IFasterSerializable<TValue>
    {
        public override void Deserialize(out TValue obj)
        {
            obj = TValue.ReadFromFaster(reader);
        }

        public override void Serialize(ref TValue obj)
        {
            obj.WriteToFaster(writer);
        }
    }
}
