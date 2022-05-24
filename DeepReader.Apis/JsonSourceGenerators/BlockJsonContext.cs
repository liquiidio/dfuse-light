using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeepReader.Types.StorageTypes;

namespace DeepReader.Apis.JsonSourceGenerators
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        GenerationMode = JsonSourceGenerationMode.Serialization)]
    [JsonSerializable(typeof(Block))]
    internal partial class BlockJsonContext : JsonSerializerContext
    {
    }
}
