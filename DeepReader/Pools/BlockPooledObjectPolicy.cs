using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using KGySoft.CoreLibraries;
using Microsoft.Extensions.ObjectPool;

namespace DeepReader.Pools
{
    internal class BlockPooledObjectPolicy : IPooledObjectPolicy<Block>
    {
        public BlockPooledObjectPolicy()
        {
        }

        public Block Create()
        {
            return new Block();
        }

        public bool Return(Block obj)
        {
            // reset Block to defaults
            obj.Id = BlockId.TypeEmpty;
            obj.Number = 0;
            obj.Version = 0;
            obj.Header = null;
            obj.ProducerSignature = Signature.TypeEmpty;
            obj.BlockExtensions = Array.Empty<Extension>();
            obj.DposProposedIrreversibleBlocknum = 0;
            obj.DposIrreversibleBlocknum = 0;
            obj.BlockrootMerkle = null;
            obj.ProducerToLastProduced = Array.Empty<PairAccountNameBlockNum>();
            obj.ProducerToLastImpliedIrb = Array.Empty<PairAccountNameBlockNum>();
            obj.ConfirmCount = Array.Empty<byte>();
            obj.PendingSchedule = null;
            obj.ActivatedProtocolFeatures = null;
            obj.Validated = false;
            obj.RlimitOps.Clear();
            obj.UnfilteredTransactions.Clear();
            obj.UnfilteredImplicitTransactionOps.Clear();
            obj.UnfilteredTransactionTraces.Clear();
            obj.BlockSigningKey = PublicKey.TypeEmpty;
            obj.ValidBlockSigningAuthority = null;
            obj.ActiveSchedule = null;

            return true;
        }
    }
}
