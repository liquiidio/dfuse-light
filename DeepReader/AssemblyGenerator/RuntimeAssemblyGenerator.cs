using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Helpers;
using Serilog;
using Path = System.IO.Path;

namespace DeepReader.AssemblyGenerator;

public static class RuntimeAssemblyGenerator
{
    public static string AssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAssemblies");

    public static async Task CreateAbiAndAssemblyAsync(Name contractName, byte[] abiBytes, uint blockNum,
        CancellationToken cancellationToken)
    {
        await Task.Run(() => CreateAbiAndAssembly(contractName, abiBytes, blockNum), cancellationToken);
    }

    public static void CreateAbiAndAssembly(Name contractName, byte[] abiBytes, uint blockNum)
    {

        var abi = DeepMindDeserializer.DeepMindDeserializer.Deserialize<Abi>(abiBytes);
        CreateAssembly(contractName, abi, blockNum);
    }

    public static Assembly CreateAssembly(Name contractName, Abi abi, uint blockNum)
    {
        var abiTypeCacheEntry = new AbiTypeCacheEntry();

        var assemblyName = new AssemblyName(contractName);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("Main");

        foreach (var abiType in abi.AbiTypes)
        {
            abiTypeCacheEntry.AbiTypeTypes.TryAdd(abiType.NewTypeName, abiType.Type);
        }
        foreach (var abiStruct in abi.AbiStructs)
        {
            var recursions = 0;
            Type? type;

            if (!string.IsNullOrWhiteSpace(abiStruct.Base)) // TODO multi-inheritance
            {
                var abiStructWithBase = new AbiStruct()
                {
                    Name = abiStruct.Name
                };
                var baseStruct = abi.AbiStructs.SingleOrDefault(a => a.Name == abiStruct.Base, new AbiStruct());
                abiStructWithBase.Fields = new AbiField[baseStruct.Fields.Length + abiStruct.Fields.Length];
                var i = 0;
                foreach (var baseStructField in baseStruct.Fields)
                {
                    abiStructWithBase.Fields[i] = baseStructField;
                    i++;
                }

                foreach (var abiStructField in abiStruct.Fields)
                {
                    abiStructWithBase.Fields[i] = abiStructField;
                    i++;
                }

                type = moduleBuilder.AddTypeFromStruct(abiStructWithBase, abi, contractName,
                    ref abiTypeCacheEntry.AbiStructTypes, ref recursions);
            }
            else
            {
                type = moduleBuilder.AddTypeFromStruct(abiStruct, abi, contractName,
                    ref abiTypeCacheEntry.AbiStructTypes, ref recursions);
            }

            if (type != null)
            {
                abiTypeCacheEntry.AbiStructTypes.TryAdd(abiStruct.Name, type);
            }
        }

        if (abi.AbiActions != null)
            foreach (var abiAction in abi.AbiActions)
            {
                abiTypeCacheEntry.AbiActionTypes.TryAdd(abiAction.Name, abiAction.Type);
            }

        if (abi.AbiTables != null)
            foreach (var abiTable in abi.AbiTables)
            {
                abiTypeCacheEntry.AbiTableTypes.TryAdd(abiTable.Name, abiTable.Type);
            }

        SaveAssemblyAndAbi(moduleBuilder.Assembly, abi, contractName, blockNum);

        Log.Information("Assemblies for contract [" + contractName + "] at block " + blockNum + " generated");

        AssemblyCache.UpsertAbiTypeCacheEntry(contractName, abiTypeCacheEntry, blockNum);

        return moduleBuilder.Assembly;
    }

    private static async void SaveAssemblyAndAbi(Assembly assembly, Abi abi, string contractName, uint blockNum)
    {
        var generator = new Lokad.ILPack.AssemblyGenerator();

        var path = Path.Combine(AssemblyPath, contractName);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        await using (var fileStream = File.CreateText(Path.Combine(path, contractName + "." + blockNum + ".abi")).BaseStream)
        {
            await JsonSerializer.SerializeAsync(fileStream, abi, typeof(Abi));
        }
        generator.GenerateAssembly(assembly, Path.Combine(path, contractName + "." + blockNum + ".dll"));
    }

    // TODO max-nesting?
    private static Type? AddTypeFromStruct(this ModuleBuilder moduleBuilder, AbiStruct abiStruct, Abi abi,
        string contractName, ref ConcurrentDictionary<string, Type> abiStructTypes, ref int recursions)
    {
        if (recursions == 4)
            return null;
        recursions++;

        var fields = new List<Tuple<string, Type, bool>>();

        Type? structType;

        if ((structType = moduleBuilder.GetType(contractName + "." + abiStruct.Name)) == null)
        {
            foreach (var field in abiStruct.Fields)
            {
                Type? type;

                var isArrayType = false;
                var isOptional = false;
                var isOptionalReferenceType = false;

                var fieldTypeName = field.Type;
                if (fieldTypeName.Contains("[]"))
                {
                    fieldTypeName = fieldTypeName.Replace("[]", ""); // TODO substring or remove is faster
                    isArrayType = true;
                }
                else if (fieldTypeName.Contains("$"))
                {
                    fieldTypeName = fieldTypeName.Replace("$", ""); // TODO substring or remove is faster
                }
                else if (fieldTypeName.Contains("?"))
                {
                    fieldTypeName = fieldTypeName.Replace("?", ""); // TODO substring or remove is faster
                    isOptional = true;
                }

                var abiType = abi.AbiTypes?.FirstOrDefault(at => at.NewTypeName == fieldTypeName)?.Type;
                if (abiType != null)
                    fieldTypeName = abiType;

                if (fieldTypeName == abiStruct.Name)
                {
                    type = moduleBuilder.DefineType(contractName + "." + abiStruct.Name,
                        TypeAttributes.Public |
                        TypeAttributes.Sealed |
                        TypeAttributes.SequentialLayout |
                        TypeAttributes.Serializable,
                        typeof(ValueType));
                }
                else if (!EosTypes.TryGetValue(fieldTypeName, out type))
                {
                    type = moduleBuilder.GetType(fieldTypeName);
                    if (type == null)
                    {
                        var abiStructType = abi.AbiStructs.SingleOrDefault(s => s.Name == fieldTypeName);
                        if (abiStructType == null)
                        {       
                            Log.Error("CONTRACT:" + contractName + "abiStruct-type not found: " + fieldTypeName);
                        }
                        else
                        {
                            type = AddTypeFromStruct(moduleBuilder, abiStructType, abi, contractName,
                                ref abiStructTypes, ref recursions);
                            if (type != null)
                            {
                                abiStructTypes.TryAdd(abiStructType.Name, type);
                            }
                        }
                    }
                }

                if (type == null)
                {
                    Log.Error("type is null: " + fieldTypeName + " recursions: " + recursions);
                    return null;
                }

                if (isArrayType)
                    type = type.MakeArrayType();
                if (isOptional) //&& type.IsValueType)
                {
                    if (type.IsValueType)
                        type = typeof(Nullable<>).MakeGenericType(type);
                    else
                        isOptionalReferenceType = true;
                }

                fields.Add(new Tuple<string, Type, bool>(field.Name, type, isOptionalReferenceType));

            }

            return moduleBuilder.AddType(contractName, abiStruct.Name, fields);
        }

        return structType;
    }

    private static Type AddType(this ModuleBuilder moduleBuilder, string contractName, string typeName,
        List<Tuple<string, Type, bool>> fields)
    {
        var dynamicType = moduleBuilder.DefineType(contractName + "." + typeName,
            TypeAttributes.Public |
            TypeAttributes.Sealed |
            TypeAttributes.SequentialLayout |
            TypeAttributes.Serializable,
            typeof(ValueType));
        foreach (var (name, type, isOptionalReferenceType) in fields)
        {
            var fieldBuilder = dynamicType.DefineField(name, type, FieldAttributes.Public);
            if (isOptionalReferenceType)
                fieldBuilder.SetCustomAttribute(NullableHelper.NullableAttributeBuilder);
        }

        return dynamicType.CreateType()!;
    }

    public static Dictionary<string, Type> EosTypes = new()
    {
        {"string", typeof(string)},
        {"asset", typeof(Asset)},
        {"name", typeof(Name)},
        {"bytes", typeof(Bytes)},

        {"checksum160", typeof(Checksum160)},
        {"checksum256", typeof(Checksum256)},
        {"checksum512", typeof(Checksum512)},
        {"extension", typeof(Extension)},
        {"transaction", typeof(Transaction)},

        {"private_key", typeof(string)},
        {"public_key", typeof(PublicKey)},
        {"signature", typeof(Signature)},

        {"symbol", typeof(Symbol)},
        {"symbol_code", typeof(SymbolCode) },

        {"time_point", typeof(ulong)},
        {"time_point_sec", typeof(uint)},
        {"block_timestamp_type", typeof(uint)},

        {"int8", typeof(sbyte)},
        {"int16", typeof(short)},
        {"int32", typeof(int)},
        {"int64", typeof(long)},
        {"int128", typeof(Int128)},

        {"uint8", typeof(byte)},
        {"uint16", typeof(ushort)},
        {"uint32", typeof(uint)},
        {"uint64", typeof(ulong)},
        {"uint128", typeof(Uint128)},
        {"varuint32", typeof(VarUint32)},
        {"varuint64", typeof(VarUint64)},

        {"float32", typeof(float)},
        {"float64", typeof(double)},
        {"float128", typeof(Float128)},
        {"bool", typeof(bool)},
        {"pair_uint16_uint16", typeof(Tuple<ushort, ushort>)},
        {"producer_schedule", typeof(ProducerSchedule)},
        {"account_name", typeof(Name)},
        {"authority", typeof(Authority)},
        {"key_wait", typeof(KeyWeight)},
        {"permission_level_weight", typeof(PermissionLevelWeight)},
        {"permission_level", typeof(PermissionLevel)},
//            {"permission", typeof(Permission)},
        {"extended_asset", typeof(ExtendedAsset)},

        // TODO pair/tuple-types
    };

    //public class NullableHelper
    //{
    //    public static object? Helper;

    //    private static CustomAttributeData _nullableAttribute;

    //    public static CustomAttributeData NullableAttribute = _nullableAttribute ?? GetNullableAttribute();

    //    private static CustomAttributeData GetNullableAttribute()
    //    {
    //        _nullableAttribute = typeof(NullableHelper).GetFields()[0].CustomAttributes.FirstOrDefault(x =>
    //            x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
    //        return _nullableAttribute;
    //    }

    //    private static CustomAttributeBuilder _nullableAttributeBuilder;

    //    public static CustomAttributeBuilder NullableAttributeBuilder =
    //        _nullableAttributeBuilder ?? GetNullableAttributeBuilder();

    //    public static CustomAttributeBuilder GetNullableAttributeBuilder()
    //    {
    //        _nullableAttribute = typeof(NullableHelper).GetFields()[0].CustomAttributes.FirstOrDefault(x =>
    //            x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

    //        var nullableAttr = NullableAttribute;
    //        var constructorAttributes = nullableAttr.ConstructorArguments.Select(m => m.StringVal).ToArray();
    //        _nullableAttributeBuilder = new CustomAttributeBuilder(nullableAttr.Constructor, constructorAttributes);
    //        return _nullableAttributeBuilder;
    //    }
    //}
}