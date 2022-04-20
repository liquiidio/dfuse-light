using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Helpers;
using DeepReader.Types.Interfaces;
using Prometheus;
using Salar.BinaryBuffers;
using Serilog;

namespace DeepReader.DeepMindDeserializer;
// TODO, micro-optimization with Dynamic Methods, DynamicMethods-Cache (Dict<type,dynMethod>) and ILGenerator ?
// https://andrewlock.net/benchmarking-4-reflection-methods-for-calling-a-constructor-in-dotnet/
//
// TODO Can we use ReadOnlySpan<byte> or ReadOnlyMemory<byte> instead of byte[] and would it bring a benefit?
public static class DeepMindDeserializer
{
    // This should keep a count of the blocks created, that can be used to determine blocks per second (theoretically)
    private static readonly Counter deserializedBlocksCount = Metrics.CreateCounter("deepreader_deserialized_blocks_count", "Number of deserialized blocks");

    public static async Task<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken) where T : IEosioSerializable<T>
    {
        return await Task.Run(() => Deserialize<T>(data), cancellationToken);
    }

    public static T Deserialize<T>(byte[] data) where T : IEosioSerializable<T>
    {
        deserializedBlocksCount.Inc();
        var reader = new BinaryReader(new MemoryStream(data));
        return T.ReadFromBinaryReader(reader);
    }



//    public static object Deserialize(byte[] data, Type type)
//    {
//#if DEBUG
//        try
//        {
//#endif
//            object obj = null!;
//            try
//            {
//                var reader = new BinaryBufferReader(data);

//                if (VariantReaders.TryGetValue(type, out var variantReader))
//                {
//                    obj = variantReader(reader);
//                    if (reader.Position != data.Length && type != CommonTypes.TypeOfAbi)
//                    {
//                        Log.Error($"[{type.Name}] : reader has not read until end {reader.Position} of {data.Length} obj: {obj}");
//                    }
//                    return obj;
//                }
//                else
//                {
//                    obj = GetTypeReader(type)(reader, type);
//                    if (reader.Position != data.Length && type != CommonTypes.TypeOfAbi)
//                        Log.Error($"[{type.Name}] : reader has not read until end {reader.Position} of {data.Length} obj: {obj}");
//                    return obj;
//                }
//            }
//            catch (EndOfStreamException)
//            {
//                Log.Error($"[{type.Name}] End of stream ", "");
//                if (obj != null)
//                    Log.Information($"obj: {obj}");
//            }
//            catch (Exception e)
//            {
//                Log.Error(e, "");
//                throw;
//            }
//            return Activator.CreateInstance(type)!;
//#if DEBUG
//        }
//        catch (Exception e)
//        {
//            Log.Debug($"type: {type.Name}");
//            throw;
//        }
//#endif
//    }

    public static object ReadReferenceType(BinaryBufferReader binaryReader, Type type)
    {
#if DEBUG
        try
        {
#endif
            var obj = Activator.CreateInstance(type)!;
            var objRef = __makeref(obj);

            /*var fields = type.GetFields().All(f => Attribute.IsDefined(f, CommonTypes.TypeOfSortOrderAttribute))
                ? type.GetFields().OrderBy(f =>
                    // ReSharper disable once PossibleNullReferenceException
                    ((SortOrderAttribute) f.GetCustomAttribute(CommonTypes.TypeOfSortOrderAttribute, true)).Order).ToList()
                : type.GetFields().ToList();*/

            foreach (var fieldInfo in TryGetCachedFieldInfos(type))
            {
                var fieldType = fieldInfo.Key.FieldType;

                if (VariantReaders.TryGetValue(fieldType, out var variantReader))
                {
                    var value = fieldInfo.Value
                        ? ReadOptional(binaryReader, variantReader)
                        : variantReader(binaryReader);
                    if (value != null)
                        fieldInfo.Key.SetValueDirect(objRef, value);
                }
                else if (fieldType.IsAbstract)
                {
                    continue;
                }
                else if (fieldType.IsEnum)
                {
                    var value = ReadEnum(binaryReader);
                    fieldInfo.Key.SetValueDirect(objRef, Enum.ToObject(fieldType, value));
                }
                else
                {
                    var typeReader = GetTypeReader(fieldType);

                    var value = fieldInfo.Value
                        ? ReadOptional(binaryReader, fieldType, typeReader)
                        : typeReader(binaryReader, fieldType);

                    if (value != null)
                        fieldInfo.Key.SetValueDirect(objRef, value);
                }
            }

            return obj;
#if DEBUG
        }
        catch (Exception e)
        {
            Log.Debug($"type: {type.Name} {binaryReader.Position}");
            throw;
        }
#endif
    }

    private static readonly ConcurrentDictionary<Type, List<KeyValuePair<FieldInfo, bool>>> CachedFieldInfos = new();

    private static List<KeyValuePair<FieldInfo, bool>> TryGetCachedFieldInfos(Type type)
    {
        if (CachedFieldInfos.TryGetValue(type, out var fields))
            return fields;

        fields = type.GetFields().All(f => Attribute.IsDefined(f, CommonTypes.TypeOfSortOrderAttribute))
            ? type.GetFields(BindingFlags.Instance | BindingFlags.Public).OrderBy(f =>
                // ReSharper disable once PossibleNullReferenceException
                ((SortOrderAttribute)f.GetCustomAttribute(CommonTypes.TypeOfSortOrderAttribute, true)!).Order).Select(f => new KeyValuePair<FieldInfo, bool>(f, NullableHelper.IsNullable(f))).ToList()
            : type.GetFields().Select(f => new KeyValuePair<FieldInfo, bool>(f, NullableHelper.IsNullable(f))).ToList();

        CachedFieldInfos.TryAdd(type, fields);
        return fields;
    }

    private static ReaderDelegate GetTypeReader(Type fieldType)
    {
        ReaderDelegate typeReader;

        if (ValueTypeReaders.ContainsKey(fieldType))
        {
            typeReader = ValueTypeReaders[fieldType];
        }
        /*else if (type.IsValueType && !type.IsEnum && !type.IsPrimitive)
        {
            typeReader = ReadReferenceType;
        }*/
        else if (fieldType.IsPrimitive || fieldType.IsValueType || fieldType == CommonTypes.TypeOfString)  // primitive types
        {
            var nullableUnderlyingType = Nullable.GetUnderlyingType(fieldType);
            if (nullableUnderlyingType != null)
                fieldType = nullableUnderlyingType;

            if (PrimitiveReaders.ContainsKey(fieldType))
                typeReader = PrimitiveReaders[fieldType];
            else if (!fieldType.IsEnum && !fieldType.IsPrimitive)
                typeReader = ReadReferenceType;
            else
                throw new Exception("Primitive TypeReader for " + fieldType.Name + " not found");
        }
        else if (fieldType.IsArray) // array types
        {
            typeReader = ReadArray;
        }
        else if (fieldType.IsGenericType) // generic types IList<>, IDictionary etc.
        {
            typeReader = ReadGeneric;
        }
        else // reference types
        {
            typeReader = ReadReferenceType; //ReadReferenceType(binaryReader, fieldInfo);
        }

        return typeReader;
    }

    private delegate object ReaderDelegate(BinaryBufferReader binaryReader, Type fieldType);
    private delegate object VariantReaderDelegate(BinaryBufferReader binaryReader);


    private static int ReadEnum(BinaryBufferReader binaryReader)
    {
        return binaryReader.ReadByte();
    }

    private static object? ReadOptional(BinaryBufferReader binaryReader, Type fieldType, ReaderDelegate typeReader)
    {
        var isValue = binaryReader.ReadByte();
        return isValue == 0 ? null : typeReader(binaryReader, fieldType);
    }

    private static object? ReadOptional(BinaryBufferReader binaryReader, VariantReaderDelegate variantReader)
    {
        var isValue = binaryReader.ReadByte();
        return isValue == 0 ? null : variantReader(binaryReader);
    }

    private static object ReadArray(BinaryBufferReader binaryReader, Type type)
    {
#if DEBUG
        try
        {
#endif
            var elementType = type.GetElementType()!;
            var size = Convert.ToInt32(binaryReader.ReadVarUint32());

            var items = (Array)Activator.CreateInstance(type, size)!;
            //        var items = (Array)Activator.CreateInstance(type, new object[] { size })!;
            if (size > 0)
            {
                if (VariantReaders.TryGetValue(elementType, out var variantReader))
                {
                    for (var i = 0; i < size; i++)
                    {
                        items.SetValue(variantReader(binaryReader), i);
                    }
                }
                else
                {
                    var typeReader = GetTypeReader(elementType);

                    for (var i = 0; i < size; i++)
                    {
                        items.SetValue(typeReader(binaryReader, elementType), i);
                    }
                }
            }

            return items;
#if DEBUG
        }
        catch (Exception e)
        {
            Log.Debug($"type: {type.Name} {binaryReader.Position}");
            throw;
        }
#endif
    }

    private static object ReadGeneric(BinaryBufferReader binaryReader, Type type)
    {
#if DEBUG
        try
        {
#endif
            var genericType = type.GetGenericTypeDefinition();
            var genericArgs = type.GetGenericArguments();
            var size = Convert.ToInt32(binaryReader.ReadVarUint32());

            if (VariantReaders.TryGetValue(type, out var genTypeReader))
            {
                var generic = genTypeReader(binaryReader);
                return generic;
            }
            else if (genericType == typeof(IDictionary))
            {
                var items = (IDictionary)Activator.CreateInstance(type)!;
                if (genericArgs.Length == 2)
                {
                    var typeReader1 = GetTypeReader(genericArgs[0]);
                    var typeReader2 = GetTypeReader(genericArgs[1]);
                    for (var i = 0; i < size; i++)
                    {
                        items?.Add(typeReader1(binaryReader, genericArgs[0]),
                            typeReader2(binaryReader, genericArgs[1]));
                    }
                }
                return items!;
            }
            else if (genericType == typeof(IList))
            {
                var items = (IList)Activator.CreateInstance(type)!;
                if (genericArgs.Length == 1)
                {
                    var typeReader = GetTypeReader(genericArgs[0]);
                    for (var i = 0; i < size; i++)
                    {
                        items?.Add(typeReader(binaryReader, genericArgs[0]));
                    }
                }
                return items!;
            }
            else
            {
                Log.Information($"Generic Type {type.Name} not allowed");
            }
            return null!;
#if DEBUG
        }
        catch (Exception e)
        {
            Log.Debug($"type: {type.Name} {binaryReader.Position}");
            throw;
        }
#endif
    }

    private static readonly Dictionary<Type, ReaderDelegate> PrimitiveReaders = new()
    {
        { CommonTypes.TypeOfString, (reader, _) => reader.ReadString() },  // We know, string is not a primitive... we treat it like that anyway.
        { CommonTypes.TypeOfBoolean, (reader, _) => reader.ReadBoolean() },
        { CommonTypes.TypeOfByte, (reader, _) => reader.ReadByte() },
        { CommonTypes.TypeOfSbyte, (reader, _) => reader.ReadSByte() },
        { CommonTypes.TypeOfDecimal, (reader, _) => reader.ReadDecimal() },
        { CommonTypes.TypeOfFloat, (reader, _) => reader.ReadFloat32() },
        { CommonTypes.TypeOfDouble, (reader, _) => reader.ReadFloat64() },
        { CommonTypes.TypeOfFloat128, (reader, _) => reader.ReadFloat128() },
        { CommonTypes.TypeOfInt, (reader, _) => reader.ReadInt32() },
        { CommonTypes.TypeOfUint, (reader, _) => reader.ReadUInt32() },
        { CommonTypes.TypeOfLong, (reader, _) => reader.ReadInt64() },
        { CommonTypes.TypeOfUlong, (reader, _) => reader.ReadUInt64() },
        { CommonTypes.TypeOfShort, (reader, _) => reader.ReadInt16() },
        { CommonTypes.TypeOfUshort, (reader, _) => reader.ReadUInt16() },
    };

    private static readonly Dictionary<Type, ReaderDelegate> ValueTypeReaders = new()
    {
        { CommonTypes.TypeOfVarUint32, (reader, _) => reader.ReadVarUint32Obj() },
        { CommonTypes.TypeOfVarInt32, (reader, _) => reader.ReadVarInt32Obj() },
        { CommonTypes.TypeOfVarUint64, (reader, _) => reader.ReadVarUint64Obj() },
        { CommonTypes.TypeOfVarInt64, (reader, _) => reader.ReadVarInt64Obj() },
        { CommonTypes.TypeOfChecksum160, (reader, _) => reader.ReadChecksum160() },
        { CommonTypes.TypeOfChecksum256, (reader, _) => reader.ReadChecksum256() },
        { CommonTypes.TypeOfChecksum512, (reader, _) => reader.ReadChecksum512() },
        { CommonTypes.TypeOfSignature, (reader, _) => reader.ReadSignature() },//TODO 
        { CommonTypes.TypeOfUint128, (reader, _) => reader.ReadUInt128() },
        { CommonTypes.TypeOfInt128, (reader, _) => reader.ReadInt128() },
        { CommonTypes.TypeOfName, (reader, _) => reader.ReadName() },
        { CommonTypes.TypeOfBytes, (reader, _) => reader.ReadBytes() },
        { CommonTypes.TypeOfPublicKey, (reader, _) => reader.ReadPublicKey() },
        { CommonTypes.TypeOfActionDataBytes, (reader, _) => reader.ReadActionDataBytes() },
        { CommonTypes.TypeOfTransactionId, (reader, _) => reader.ReadTransactionId() },
        { CommonTypes.TypeOfAsset, (reader, _) => reader.ReadAsset() },
        { CommonTypes.TypeOfSymbol, (reader, _) => reader.ReadSymbol() },
        { CommonTypes.TypeOfSymbolCode, (reader, _) => reader.ReadSymbolCode() },
        { CommonTypes.TypeOfTimestamp, (reader, _) => reader.ReadTimestamp() },
    };

    private static readonly Dictionary<Type, VariantReaderDelegate> VariantReaders = new()
    {
        { CommonTypes.TypeOfBlockSigningAuthorityVariant, ReadBlockSigningAuthorityVariant },
        { CommonTypes.TypeOfTransactionVariant, ReadTransactionVariant },
    };


    #region Variants

    public static BlockSigningAuthorityVariant ReadBlockSigningAuthorityVariant(BinaryBufferReader reader)
    {
        var i = Convert.ToInt32(reader.ReadVarUint32());
        return i switch
        {
            0 => (BlockSigningAuthorityVariant)ReadReferenceType(reader, CommonTypes.TypeOfBlockSigningAuthorityV0),
            _ => throw new Exception($"BlockSigningAuthorityVariant VariantType {i} unknown")
        };
    }

    public static TransactionVariant ReadTransactionVariant(BinaryBufferReader reader)
    {
        var i = Convert.ToInt32(reader.ReadVarUint32());
        return i switch
        {
            0 => (TransactionVariant)ReadReferenceType(reader, CommonTypes.TypeOfTransactionId),
            1 => (TransactionVariant)ReadReferenceType(reader, CommonTypes.TypeOfPackedTransaction),
            _ => throw new Exception($"TransactionVariant VariantType {i} unknown")
        };
    }

    #endregion Variants
}