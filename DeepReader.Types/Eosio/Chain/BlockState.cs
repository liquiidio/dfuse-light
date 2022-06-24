using Salar.BinaryBuffers;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/block_state.hpp
/// </summary>
public sealed class BlockState : BlockHeaderState, IEosioSerializable<BlockState>
{
    public SignedBlock? Block;

    public bool Validated;

    public BlockState(BinaryBufferReader reader) : base(reader)
    {
        var readBlock = reader.ReadBoolean();

        if (readBlock)
            Block = SignedBlock.ReadFromBinaryReader(reader);

        Validated = reader.ReadBoolean();
    }

    public new static BlockState ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        return new BlockState(reader);
    }
}