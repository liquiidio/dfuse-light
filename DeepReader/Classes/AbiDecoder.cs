using System.Buffers;
using System.Text.Json;
using DeepReader.AssemblyGenerator;
using DeepReader.Storage;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
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

    private ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;
    public void AddInitialAbi(ReadOnlySpan<char> contract, ReadOnlySpan<char> rawAbiBase64)
    {
        var bytes = ArrayPool.Rent(rawAbiBase64.Length * 2);

        if (Convert.TryFromBase64Chars(rawAbiBase64, bytes, out var bytesWritten))
        {
            var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(bytes[Range.EndAt(bytesWritten)]);

            var contractAccount = NameCache.GetOrCreate(contract.ToString());
            AbiAssemblyGenerator abiAssemblyGenerator = new(abi, contractAccount, _activeGlobalSequence);
            if (abiAssemblyGenerator.TryGenerateAssembly(out var abiAssembly))
            {
                _storageAdapter.UpsertAbi(contractAccount, _activeGlobalSequence, abiAssembly!);

                Log.Information($"Deserialized Abi for {contractAccount.StringVal}");
            }
            else
                Log.Warning($"Deserialization of Abi for {contractAccount} failed");
        }
        else
        {
            Console.WriteLine($"base64chars to byte-array of Abi for {contract} failed");
        }
        ArrayPool.Return(bytes);
    }

    public void AddAbi(Name contractAccount, byte[] bytes, ulong globalSequence)
    {
        _activeGlobalSequence = globalSequence;

        var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(bytes);

        AbiAssemblyGenerator abiAssemblyGenerator = new(abi, contractAccount, _activeGlobalSequence);
        if (abiAssemblyGenerator.TryGenerateAssembly(out var abiAssembly))
        {
            _storageAdapter.UpsertAbi(contractAccount, _activeGlobalSequence, abiAssembly!);

            Log.Information($"Deserialized Abi for {contractAccount.StringVal}");
        }
        else
            Log.Warning($"Deserialization of Abi for {contractAccount} failed");
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