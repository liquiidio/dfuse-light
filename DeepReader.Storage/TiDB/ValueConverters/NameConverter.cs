using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    public class NameConverter : ValueConverter<Name, string>
    {
        public NameConverter() : base(
            v => v.StringVal,
            v => (Name)v)
        {
        }
    }
}