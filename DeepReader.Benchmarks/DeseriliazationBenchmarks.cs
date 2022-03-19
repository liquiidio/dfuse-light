using BenchmarkDotNet.Attributes;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace DeepReader.Benchmarks
{
    [MemoryDiagnoser]
    [RankColumn]
    public class DeseriliazationBenchmarks
    {
        [Benchmark]
        public BlockHeader? Direct()
        {
            return BlockHeader.ReadFromBinaryReader();
        }

        [Benchmark]
        public BlockHeader? UsingNew()
        {
            var instance = new BlockHeader() as IParent<BlockHeader>;
            return instance.ReadFromBinaryReader();
        }

        [Benchmark]
        public BlockHeader UsingReflection()
        {
            var constructor = typeof(BlockHeader).GetConstructor(System.Type.EmptyTypes);
            var instance = constructor.Invoke(null) as IParent<BlockHeader>;
            return instance.ReadFromBinaryReader();
        }

        [Benchmark]
        public BlockHeader UsingFormatterServices()
        {
            var instance = FormatterServices.GetUninitializedObject(typeof(BlockHeader)) as IParent<BlockHeader>;
            return instance.ReadFromBinaryReader();
        }

        [Benchmark]
        public BlockHeader UsingActivatorCreateInstance()
        {
            var instance = Activator.CreateInstance(typeof(BlockHeader)) as IParent<BlockHeader>;
            return instance.ReadFromBinaryReader();
        }

        [Benchmark]
        public BlockHeader UsingCompiledExpression()
        {
            NewExpression constructorExpression = Expression.New(typeof(BlockHeader).GetConstructor(System.Type.EmptyTypes));
            Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(constructorExpression);
            Func<object> createHeadersFunc = lambdaExpression.Compile();
            var instance = createHeadersFunc() as IParent<BlockHeader>;
            return instance.ReadFromBinaryReader();
        }

        [Benchmark]
        public BlockHeader UsingReflectionEmit()
        {
            var typeToCreate = typeof(BlockHeader);
            DynamicMethod createDeserializeObjectMethod = new DynamicMethod(
                name: $"CreateDeserializeObject",
                returnType: typeToCreate,
                parameterTypes: null,
                m: typeof(DeseriliazationBenchmarks).Module,
                skipVisibility: false);

            var constructor = typeToCreate.GetConstructor(System.Type.EmptyTypes);
            ILGenerator il = createDeserializeObjectMethod.GetILGenerator();
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);

            var deserializationObjectDelegate = createDeserializeObjectMethod.CreateDelegate(typeof(Func<object>));
            var instance = deserializationObjectDelegate.DynamicInvoke() as IParent<BlockHeader>;

            return instance.ReadFromBinaryReader();
        }
    }
}