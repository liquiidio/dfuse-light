using DeepReader.Storage;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;
using DeepReader.Types.StorageTypes;
using Serilog;
using System.Text.Json;

namespace DeepReader.Apis.Other
{
    internal static class ActionTraceDeserializer
    {
        private static readonly Name Eosio = NameCache.GetOrCreate("eosio");

        private static readonly JsonSerializerOptions ActionTraceSerializerSettings = new JsonSerializerOptions()
        {
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            MaxDepth = Int32.MaxValue,
            WriteIndented = true,
        };

        private static readonly ParallelOptions ActionTraceSerializerParallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 3
        };

        public static async Task DeserializeActions(List<ActionTrace> actionTraces, IStorageAdapter storage)
        {
            await Parallel.ForEachAsync(actionTraces, ActionTraceSerializerParallelOptions, async (actionTrace, _) =>
            {
                await DeserializeAction(actionTrace, storage);
            });
        }

        public static async Task DeserializeAction(ActionTrace actionTrace, IStorageAdapter storage)
        {
            if (actionTrace.Act.Data.Json != null) // Don't do the same job twice
                return;

            var clrTypename = "";
            var actName = "";
            try
            {
                var (found, assemblyPair) = await storage.TryGetAbiAssemblyByIdAndGlobalSequence(actionTrace.Act.Account, actionTrace.GlobalSequence);
                if (found)  // TODO check GlobalSequence
                {
                    Log.Information($"Abi-Sequence: {assemblyPair.Key} + ActionTrace-Sequence: {actionTrace.GlobalSequence}");

                    actName = actionTrace.Act.Name;
                    var assembly = assemblyPair.Value;
                    var clrType = assembly.Assembly.GetType($"_{actionTrace.Act.Name.StringVal.Replace('.', '_')}");
                    if (clrType != null)
                    {
                        var reader = new BinaryReader(new MemoryStream(actionTrace.Act.Data));

                        clrTypename = clrType.Name;
                        var obj = Activator.CreateInstance(clrType, reader);
                        actionTrace.Act.Data.Json = JsonSerializer.SerializeToElement(obj, clrType, ActionTraceSerializerSettings);
                    }
                    else if(actionTrace.Act.Account == Eosio) // EOSIO global_sequence 0 contains system-actions like onblock
                    {
                        (found, assemblyPair) = await storage.TryGetAbiAssemblyByIdAndGlobalSequence(actionTrace.Act.Account, 0);
                        if (found)  // TODO check GlobalSequence
                        {
                            assembly = assemblyPair.Value;
                            clrType = assembly.Assembly.GetType($"_{actionTrace.Act.Name.StringVal.Replace('.', '_')}");
                            if (clrType != null)
                            {
                                var reader = new BinaryReader(new MemoryStream(actionTrace.Act.Data));

                                clrTypename = clrType.Name;
                                var obj = Activator.CreateInstance(clrType, reader);
                                actionTrace.Act.Data.Json = JsonSerializer.SerializeToElement(obj, clrType, ActionTraceSerializerSettings);
                            }
                        }
                        else
                            Log.Information($"Type for {actionTrace.Act.Account.StringVal}.{actionTrace.Act.Name.StringVal} not found");
                    }
                    else
                        Log.Information($"Type for {actionTrace.Act.Account.StringVal}.{actionTrace.Act.Name.StringVal} not found");
                
                }
                else
                    Log.Information($"Abi for {actionTrace.Act.Account.StringVal}.{actionTrace.GlobalSequence} not found");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                Log.Information(clrTypename);
                Log.Information(actName);
            }
        }
    }
}
