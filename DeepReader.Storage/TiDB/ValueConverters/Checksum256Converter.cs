using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class Checksum256Converter : ValueConverter<Checksum256, byte[]>
    {
        public Checksum256Converter() : base(
            v => v.Binary,
            v => (Checksum256)v)
        {

        }
    }
}