namespace DeepReader.Types.Other
{
    public sealed class CreationTreeNode
    {
        public CreationOpKind Kind;
        public int ActionIndex;
        public IList<CreationTreeNode> Children;
    }
}
