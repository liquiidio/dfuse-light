using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class ReadOnlyMemoryCharConverter : ValueConverter<ReadOnlyMemory<char>, string>
    {
        public ReadOnlyMemoryCharConverter() : base(
            v => new String(v.ToArray()),
            v => new ReadOnlyMemory<char>(v.ToCharArray()))
        {

        }
    }
}