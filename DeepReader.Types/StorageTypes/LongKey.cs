using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.StorageTypes
{
    public class LongKey : IKey<long>
    {
        public static implicit operator LongKey(long key)
        {
            return new LongKey();
        }

        public static void SerializeKey(long key, IBufferWriter writer)
        {
            writer.Write(key);
        }

        public static long DeserializeKey(IBufferReader reader)
        {
            return reader.ReadInt64();
        }

        public bool Equals(long key)
        {
            return this.Equals(key);
        }
    }
}
