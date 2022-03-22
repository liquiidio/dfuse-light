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
    public bool Validated = false;

    public new static BlockState ReadFromBinaryReader(BinaryReader reader)
    {
        var blockState = (BlockState)BlockHeaderState.ReadFromBinaryReader(reader);

        var readBlock = reader.ReadBoolean();

        if (readBlock)
            blockState.Block = SignedBlock.ReadFromBinaryReader(reader);
        
        blockState.Validated = reader.ReadBoolean();
        return blockState;
    }
}