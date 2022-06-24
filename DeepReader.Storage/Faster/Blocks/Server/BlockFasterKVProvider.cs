using DeepReader.Storage.Faster.Blocks.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;
using FASTER.server;

namespace DeepReader.Storage.Faster.Blocks.Server
{
    internal class BlockFasterKVProvider : FasterKVProviderBase<long, Block, BlockInput,
    BlockOutput, BlockServerFunctions, BlockServerSerializer>
    {
        public BlockFasterKVProvider(FasterKV<long, Block> store, BlockServerSerializer serializer,
       SubscribeKVBroker<long, Block, BlockInput, IKeyInputSerializer<long, BlockInput>> kvBroker = null,
       SubscribeBroker<long, Block, IKeySerializer<long>> broker = null, bool recoverStore = false,
       MaxSizeSettings maxSizeSettings = null)
       : base(store, serializer, kvBroker, broker, recoverStore, maxSizeSettings)
        {
        }

        public override BlockServerFunctions GetFunctions() => new();
    }
}