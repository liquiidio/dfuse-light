using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.StorageTypes
{
    public class UlongKey : IKey<ulong>
    {
        public static void SerializeKey(ulong key, IBufferWriter writer)
        {
            writer.Write(key);
        }

        public static ulong DeserializeKey(IBufferReader reader)
        {
            return reader.ReadUInt64();
        }

        public bool Equals(ulong key)
        {
            return this.Equals(key);
        }
    }
}
