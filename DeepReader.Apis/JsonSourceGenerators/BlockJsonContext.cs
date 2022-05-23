using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeepReader.Types;
using DeepReader.Types.FlattenedTypes;

namespace DeepReader.Apis.JsonSourceGenerators
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        GenerationMode = JsonSourceGenerationMode.Serialization)]
    [JsonSerializable(typeof(FlattenedBlock))]
    internal partial class BlockJsonContext : JsonSerializerContext
    {
    }
}
