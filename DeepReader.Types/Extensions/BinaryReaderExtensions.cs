using DeepReader.Types.Infrastructure.BinaryReaders;
using System.Text;

namespace DeepReader.Types.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static Extension ReadExtension(this IBufferReader reader)
        {
            return new KeyValuePair<ushort, byte[]>(reader.ReadUInt16(), reader.ReadBytes(reader.Read7BitEncodedInt()));
        }

        public static string ReadString(this IBufferReader reader)
        {
            var length = reader.Read7BitEncodedInt();// Convert.ToInt32(reader.ReadVarLength<int>());
            return length > 0 ? Encoding.UTF8.GetString(reader.ReadBytes(length)) : string.Empty;
        }

        public enum KeyType
        {
            K1 = 0,
            R1 = 1,
            WA = 2,
        };
    }
}
