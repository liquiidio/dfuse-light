using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.StorageAdapters
{
    public interface IStorageAdapter
    {
        void StoreBlock();

        void StoreTransation();
    }
}
