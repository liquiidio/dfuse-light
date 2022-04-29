using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Classes
{
    internal class BlockSegmentListPooledObjectPolicy : IPooledObjectPolicy<List<IList<StringSegment>>>
    {
        private readonly int _initialListSize;
        public BlockSegmentListPooledObjectPolicy(int initialListSize)
        {
            _initialListSize = initialListSize;
        }

        public List<IList<StringSegment>> Create()
        {
            return new List<IList<StringSegment>>(_initialListSize);
        }

        public bool Return(List<IList<StringSegment>> obj)
        {
            obj.Clear(); // we just want to clear the List here
            return true;
        }
    }
}
