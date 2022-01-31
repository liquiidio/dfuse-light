using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SortOrderAttribute : Attribute
    {
        public int Order { get; }

        public SortOrderAttribute(int order = 0)
        {
            Order = order;
        }
    }
}
