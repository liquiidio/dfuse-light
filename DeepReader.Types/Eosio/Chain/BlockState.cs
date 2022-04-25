using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_state.hpp
/// </summary>
public class BlockState : BlockHeaderState, IEosioSerializable<BlockState>
{
    [SortOrder(15)]
    public SignedBlock? Block;
    [SortOrder(16)]
    public bool Validated;

    public BlockState(BinaryReader reader) : base(reader)
    {
        var readBlock = reader.ReadBoolean();

        if (readBlock)
            Block = SignedBlock.ReadFromBinaryReader(reader);

        Validated = reader.ReadBoolean();
    }

    public new static BlockState ReadFromBinaryReader(BinaryReader reader)
    {
        return new BlockState(reader);
    }
}