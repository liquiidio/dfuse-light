using BenchmarkDotNet.Attributes;
using System.Runtime.Versioning;

namespace DeepReader.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn]
    public class DeseriliazationBenchmarks
    {
        [Benchmark]
        public BlockHeader DeserializeBlockHeader()
        {
            return Deserialize<BlockHeader>();
        }

        [RequiresPreviewFeatures]
        public static T Deserialize<T>() where T : IParent<T>
        {
            return T.ReadFromBinaryReader();
        }
    }
}