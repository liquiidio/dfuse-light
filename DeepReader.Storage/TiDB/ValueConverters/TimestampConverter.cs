using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class TimestampConverter : ValueConverter<Timestamp, uint>
    {
        public TimestampConverter() : base(
            v => v.Ticks,
            v => new Timestamp(v))
        {

        }
    }
}