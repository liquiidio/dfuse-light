using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class ULongArrayConverter : ValueConverter<ulong[], string>
    {
        public ULongArrayConverter() : base(
            v => String.Join(',', v),
            v => Array.ConvertAll(v.Split(',', StringSplitOptions.RemoveEmptyEntries), ulong.Parse))
        {

        }
    }
}