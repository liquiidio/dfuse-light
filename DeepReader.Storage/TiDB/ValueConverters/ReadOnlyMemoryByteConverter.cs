using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    public class ReadOnlyMemoryByteConverter : ValueConverter<ReadOnlyMemory<byte>, byte[]>
    {
        public ReadOnlyMemoryByteConverter() : base(
            v => v.ToArray(),
            v => new ReadOnlyMemory<byte>(v))
        {

        }
    }
}
