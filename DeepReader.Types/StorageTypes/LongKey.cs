using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.StorageTypes
{
    internal class LongKey : IKey<long>
    {
        public static void SerializeKey(long key, BinaryWriter writer)
        {
            writer.Write(key);
        }

        public static long DeserializeKey(IBufferReader reader)
        {
            return reader.ReadInt64();
        }

        public static long DeserializeKey(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        public bool Equals(long key)
        {
            return this.Equals(key);
        }
    }
}
