using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.Interfaces
{
    public interface IKey<TKey>
        {

            // need to pass TKey here because can't inherit primitives
            static abstract void SerializeKey(TKey key, IBufferWriter writer);

            static abstract TKey DeserializeKey(IBufferReader reader);

            bool Equals(TKey key);
        }
}
