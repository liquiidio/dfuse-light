using System.Text.Json;
using DeepReader.AssemblyGenerator;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using Serilog;

namespace DeepReader.Classes;

public class AbiDecoder
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };

    public static void ProcessTransactionTrace(TransactionTrace trace)
    {
        // TODO
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
    }

    public static void EndBlock(Block block)
    {
        // TODO
    }

    public static void ResetCache()
    {
        // TODO
    }

    public static void AddInitialAbi(ReadOnlySpan<char> contract, ReadOnlySpan<char> rawAbiBase64)
    {
        byte[] bytes = new byte[rawAbiBase64.Length*2]; // TODO calculate bytes-size

        if (Convert.TryFromBase64Chars(rawAbiBase64, bytes, out var bytesWritten))
        {
            Console.WriteLine($"bytesWritten " + bytesWritten + " rawAbiBase64.Length " + rawAbiBase64.Length); // TODO remove
            var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(bytes[Range.EndAt(bytesWritten)]);
            Log.Information($"Deserialized Abi for {contract} : {JsonSerializer.Serialize(abi, JsonSerializerOptions)}");
        }
        else
        {
            Console.WriteLine($"Deserialization of Abi for {contract} FAILED");
        }
    }
}