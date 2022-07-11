using DeepReader.Storage.Faster.Abis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class AbiVersionsConverter : ValueConverter<SortedDictionary<ulong, AssemblyWrapper>, string>
    {
        public AbiVersionsConverter() : base(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
            v => JsonSerializer.Deserialize<SortedDictionary<ulong, AssemblyWrapper>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General))!)
        {

        }
    }
}