using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Interfaces;
using FASTER.core;

namespace DeepReader.Storage.Faster.Base.Standalone
{
    public class IdSerializer<TKey> : BinaryObjectSerializer<TKey>
    where TKey : IKey<TKey>
    {
        public override void Deserialize(out TKey obj)
        {
            obj = TKey.DeserializeKey(reader);
        }

        public override void Serialize(ref TKey obj)
        {
            TKey.SerializeKey(obj, writer);
        }
    }
    }
