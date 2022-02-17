using DeepReader.Types;
using DeepReader.Types.Enums;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.FlattenedTypes;
using Action = DeepReader.Types.Eosio.Chain.Action;

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
                flattenedTransactionTrace.ActionTraces = new FlattenedActionTrace[unflattenedTransactionTrace.ActionTraces.Length];
                for (int i = 0; i < unflattenedTransactionTrace.ActionTraces.Length; i++)
                {
                    var unflattenedActionTrace = unflattenedTransactionTrace.ActionTraces[i];
                    var flattenedActionTrace = new FlattenedActionTrace();



                    flattenedTransactionTrace.ActionTraces[i] = new FlattenedActionTrace();
                }

                //unflattenedTransactionTrace.
            }

            
            
            return flattenedTransactionTraces;
        }
    }
}
