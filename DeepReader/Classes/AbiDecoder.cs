using System.Text.Json;
using DeepReader.AssemblyGenerator;
using DeepReader.Storage;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Other;
using Serilog;

namespace DeepReader.Classes;

public class AbiDecoder
{
    private IStorageAdapter _storageAdapter;

    private int _activeBlockNum = 0;

    private ulong _activeGlobalSequence = 0;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
    };


    public AbiDecoder(IStorageAdapter storageAdapter)
    {
        _storageAdapter = storageAdapter;
    }

    public static void ProcessTransactionTrace(TransactionTrace trace)
    {
        // TODO
    }

    public static async void ProcessSignedTransaction(SignedTransaction signedTransaction)
    {
        //await Parallel.ForEachAsync(signedTransaction.Actions, async (action, token) => 
        //{
        //    if (AssemblyCache.ContractAssemblyCache.TryGetValue(action.Account, out var contractTypes) &&
        //        contractTypes.Last().Value.TryGetActionType(action.Name, out var actionType))
        //    {
        //        try
        //        {
        //            await action.Data.DeserializeAsync(actionType, token);
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Error(e, "");
        //            if (AssemblyCache.ContractAssemblyCache.TryRemove(action.Account, out contractTypes))
        //                Log.Information(action.Account + " removed from AssemblyCache");
        //        }
        //    }
        //});
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

    public void AddInitialAbi(ReadOnlySpan<char> contract, ReadOnlySpan<char> rawAbiBase64)
    {
        byte[] bytes = new byte[rawAbiBase64.Length*2]; // TODO calculate bytes-size

        if (Convert.TryFromBase64Chars(rawAbiBase64, bytes, out var bytesWritten))
        {
            Console.WriteLine($"bytesWritten " + bytesWritten + " rawAbiBase64.Length " + rawAbiBase64.Length); // TODO remove
            var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(bytes[Range.EndAt(bytesWritten)]);

            var contractAccount = NameCache.GetOrCreate(contract.ToString());
            AbiAssemblyGenerator abiAssemblyGenerator = new(abi, contractAccount, 0);
            var abiAssembly = abiAssemblyGenerator.GenerateAssembly();

            _storageAdapter.UpsertAbi(contractAccount, _activeGlobalSequence, abiAssembly);

            Log.Information($"Deserialized Abi for {contract} : {JsonSerializer.Serialize(abi, JsonSerializerOptions)}");
        }
        else
        {
            Console.WriteLine($"Deserialization of Abi for {contract} FAILED");
        }
    }

    internal void AbiDumpStart(int blockNum, ulong globalSequence)
    {
        _activeBlockNum = blockNum;
        _activeGlobalSequence = globalSequence;
    }

    internal void AbiDumpEnd()
    {
        _activeBlockNum = 0;
        _activeGlobalSequence = 0;
    }
}