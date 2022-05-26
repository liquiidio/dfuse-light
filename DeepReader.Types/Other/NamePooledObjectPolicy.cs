using DeepReader.Types.EosTypes;
using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Types.Other
{
    internal class NamePooledObjectPolicy : IPooledObjectPolicy<Name>
    {
        public NamePooledObjectPolicy()
        {
        }

        public Name Create()
        {
            return new Name();
        }

        public bool Return(Name obj)
        {
            obj.Clear();
            return true;
        }
    }
}
