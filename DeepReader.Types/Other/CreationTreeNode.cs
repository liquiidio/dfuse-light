using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.Other
{
    public class CreationTreeNode
    {
        public CreationOpKind Kind;
        public int ActionIndex;
        public IList<CreationTreeNode> Children;
    }
}
