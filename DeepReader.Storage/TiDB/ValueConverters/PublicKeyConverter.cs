using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class PublicKeyConverter : ValueConverter<PublicKey, string>
    {
        public PublicKeyConverter() : base(
            v => v.StringVal,
            v => (PublicKey)v)
        {

        }
    }
}