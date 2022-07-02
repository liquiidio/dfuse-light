using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.StorageTypes
{
    internal class UlongKey : IKey<ulong>
    {
        public static void SerializeKey(ulong key, BinaryWriter writer)
        {
            writer.Write(key);
        }

        public static ulong DeserializeKey(IBufferReader reader)
        {
            return reader.ReadUInt64();
        }

        public static ulong DeserializeKey(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        public bool Equals(ulong key)
        {
            return this.Equals(key);
        }
    }
}
