public class BlockHeader : IParent<BlockHeader>
{
    public string Name { get; set; } = "Haron Kipkorir";

    public static BlockHeader ReadFromBinaryReader()
    {
        return new BlockHeader();
    }
}
