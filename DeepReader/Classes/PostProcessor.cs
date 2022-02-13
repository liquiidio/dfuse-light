using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.HostedServices;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Classes
{
    internal class PostProcessor
    {
        static List<FlattenedTransactionTrace> Flatten(Block block)
        {
            var flattenedTransactionTraces = new List<FlattenedTransactionTrace>();
            foreach (var unflattenedTransactionTrace in block.UnfilteredTransactionTraces)
            {
                var flattenedTransactionTrace = new FlattenedTransactionTrace();
                flattenedTransactionTrace.FlattenedActionTraces = new FlattenedActionTrace[unflattenedTransactionTrace.ActionTraces.Length];
                for (int i = 0; i < unflattenedTransactionTrace.ActionTraces.Length; i++)
                {
                    var unflattenedActionTrace = unflattenedTransactionTrace.ActionTraces[i];
                    var flattenedActionTrace = new FlattenedActionTrace();



                    flattenedTransactionTrace.FlattenedActionTraces[i] = new FlattenedActionTrace();
                }

                //unflattenedTransactionTrace.
            }

            
            
            return flattenedTransactionTraces;
        }

        public struct FlattenedTransactionTrace
        {
            public FlattenedActionTrace[] FlattenedActionTraces = Array.Empty<FlattenedActionTrace>();
        }

        public struct FlattenedActionTrace
        {

        }

        public struct FlattenedBlock
        {
            public TransactionId[] TransactionIds = Array.Empty<TransactionId>();
        }
    }
}
