namespace DeepReader.Types.Helpers;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SortOrderAttribute : Attribute
{
    public int Order { get; }

    public SortOrderAttribute(int order = 0)
    {
        Order = order;
    }
}