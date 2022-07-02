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
    public class KeySerializer<TKey, TKKey> : BinaryObjectSerializer<TKKey>
    where TKey : IKey<TKKey>
    {
        public override void Deserialize(out TKKey obj)
        {
            obj = TKey.DeserializeKey((IBufferReader)reader);
        }

        public override void Serialize(ref TKKey obj)
        {
            TKey.SerializeKey(obj, (IBufferWriter)writer);
        }
    }
    }
