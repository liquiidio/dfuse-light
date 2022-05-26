using DeepReader.Types.StorageTypes;
using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Pools
{
    internal class ActionTraceListPooledObjectPolicy : IPooledObjectPolicy<List<ActionTrace>>
    {
        public ActionTraceListPooledObjectPolicy()
        {
        }

        public List<ActionTrace> Create()
        {
            return new List<ActionTrace>();
        }

        public bool Return(List<ActionTrace> obj)
        {
            obj.Clear();

            return true;
        }
    }
}
