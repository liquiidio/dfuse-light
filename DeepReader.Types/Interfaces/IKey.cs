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
            static abstract void SerializeKey(TKey key, BinaryWriter writer);

            static abstract TKey DeserializeKey(IBufferReader reader);

            static abstract TKey DeserializeKey(BinaryReader reader);

            bool Equals(TKey key);
        }
}
