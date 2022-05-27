using System.Text.Json.Serialization;
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
