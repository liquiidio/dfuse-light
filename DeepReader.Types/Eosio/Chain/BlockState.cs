using DeepReader.Types.Helpers;
using DeepReader.Types.Eosio.Chain.Legacy;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_state.hpp
/// </summary>
public class BlockState : BlockHeaderState
{
    [SortOrder(15)]
    public SignedBlock? Block;
    [SortOrder(16)]
    public bool Validated = false;

    public new static BlockState ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = (BlockState)BlockHeaderState.ReadFromBinaryReader(reader);
        obj.Block = SignedBlock.ReadFromBinaryReader(reader);
        obj.Validated = reader.ReadBoolean();
        return obj;
    }
}