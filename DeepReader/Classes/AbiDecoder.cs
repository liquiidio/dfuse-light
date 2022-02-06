using DeepReader.AssemblyGenerator;
using DeepReader.Types;
using Serilog;

namespace DeepReader.Classes;

public class AbiDecoder
{
    public static void ProcessTransactionTrace(TransactionTrace trace)
    {
        // TODO
        return;
    }

    public static async void ProcessSignedTransaction(SignedTransaction signedTransaction)
    {
        await Parallel.ForEachAsync(signedTransaction.Actions, async (action, token) => 
        {
            if (AssemblyCache.ContractAssemblyCache.TryGetValue(action.Account, out var contractTypes) &&
                contractTypes.Last().Value.TryGetActionType(action.Name, out var actionType))
            {
                try
                {
                    await action.Data.DeserializeAsync(actionType, token);
                }
                catch (Exception e)
                {
                    Log.Error(e, "");
                    if (AssemblyCache.ContractAssemblyCache.TryRemove(action.Account, out contractTypes))
                        Log.Information(action.Account + " removed from AssemblyCache");
                }
            }
        });
    }

    public static void StartBlock(long blockNum)
    {
        // TODO
        return;
    }

    public static void EndBlock(Block block)
    {
        // TODO
        return;
    }

    public static void ResetCache()
    {
        // TODO
        return;
    }

    public static void AddInitialABI(string contract, string rawAbi)
    {
        // TODO
        return;
    }
}