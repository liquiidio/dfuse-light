using DeepReader.Types.Fc.Crypto;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class SignatureConverter : ValueConverter<Signature, string>
    {
        public SignatureConverter() : base(
            v => v.StringVal,
            v => (Signature)v)
        {

        }
    }
}