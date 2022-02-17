using System.Text.Json;
using DeepReader.AssemblyGenerator;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using Serilog;

namespace DeepReader.Classes;

public class AbiDecoder
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

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

    public static void AddInitialAbi(string contract, string rawAbiBase64)
    {
        var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(rawAbiBase64.Base64StringToByteArray());
        Log.Information($"Deserialized Abi for {contract} : {JsonSerializer.Serialize(abi, _jsonSerializerOptions)}");
        // TODO
        return;
    }
}