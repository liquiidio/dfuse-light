using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class CharArrayConverter : ValueConverter<char[], string>
    {
        public CharArrayConverter() : base(
            v => new string(v),
            v => v.ToCharArray())
        {

        }
    }
}