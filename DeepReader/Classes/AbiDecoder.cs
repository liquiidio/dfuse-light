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
    private readonly IStorageAdapter _storageAdapter;

    public AbiDecoder(IStorageAdapter storageAdapter)
    {
        _storageAdapter = storageAdapter;
    }

    public static void ProcessTransactionTrace(TransactionTrace trace)
    {
        // REMOVE
    }

    public static void ProcessSignedTransaction(SignedTransaction signedTransaction)
    {
        // REMOVE
    }

    public static void StartBlock(long blockNum)
    {
        // REMOVE
    }

    public static void EndBlock(Block block)
    {
        // REMOVE
    }

    public static void ResetCache()
    {
        // REMOVE
    }

    private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

    public void AddInitialAbi(ReadOnlySpan<char> contract, ReadOnlySpan<char> rawAbiBase64, ulong globalSequence)
    {
        var bytes = _arrayPool.Rent(rawAbiBase64.Length * 2);
        try
        {
            if (Convert.TryFromBase64Chars(rawAbiBase64, bytes, out var bytesWritten))
            {
                var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(bytes[Range.EndAt(bytesWritten)]);

                var contractAccount = NameCache.GetOrCreate(contract.ToString());
                AbiAssemblyGenerator abiAssemblyGenerator = new(abi, contractAccount, globalSequence);
                if (abiAssemblyGenerator.TryGenerateAssembly(out var abiAssembly))
                {
                    _storageAdapter.UpsertAbi(contractAccount, globalSequence, abiAssembly!);

                    Log.Information($"Deserialized Abi for {contractAccount.StringVal}");
                }
                else
                    Log.Warning($"Deserialization of Abi for {contractAccount} failed");
            }
            else
            {
                Console.WriteLine($"base64chars to byte-array of Abi for {contract} failed");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _arrayPool.Return(bytes);
        }
    }

    public void AddAbi(Name contractAccount, byte[] bytes, ulong globalSequence)
    {
        var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(bytes);

        AbiAssemblyGenerator abiAssemblyGenerator = new(abi, contractAccount, globalSequence);
        if (abiAssemblyGenerator.TryGenerateAssembly(out var abiAssembly))
        {
            _storageAdapter.UpsertAbi(contractAccount, globalSequence, abiAssembly!);

            Log.Information($"Deserialized Abi for {contractAccount.StringVal}");
        }
        else
            Log.Warning($"Deserialization of Abi for {contractAccount} failed");
    }

    internal void AbiDumpStart(int blockNum, ulong globalSequence)
    {
        // REMOVE
    }

    internal void AbiDumpEnd()
    {
        // REMOVE
    }
}