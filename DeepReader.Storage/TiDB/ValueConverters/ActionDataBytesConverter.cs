using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class ActionDataBytesConverter : ValueConverter<ActionDataBytes, byte[]>
    {
        public ActionDataBytesConverter() : base(
            v => v.Binary,
            v => (ActionDataBytes)v)
        {

        }
    }
}