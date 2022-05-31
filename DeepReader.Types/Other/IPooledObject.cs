using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Types.Other
{
    public abstract class PooledObject<T> where T : class, new()
    {
        protected static readonly ObjectPool<T> TypeObjectPool = new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T>());

        public static T FromPool()
        {
            return TypeObjectPool.Get();
        }

        public static void ReturnToPool(T obj)
        {
            TypeObjectPool.Return(obj);
        }
    }

    public interface IParentPooledObject<in T> where T : PooledObject<T>,  new()
    {
        void ReturnToPoolRecursive();
    }

}
