using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using Salar.BinaryBuffers;
using Serilog;

namespace DeepReader.Deserializer
{
    // TODO, micro-optimization with Dynamic Methods, DynamicMethods-Cache (Dict<type,dynMethod>) and ILGenerator ?
    // https://andrewlock.net/benchmarking-4-reflection-methods-for-calling-a-constructor-in-dotnet/

    public static class Deserializer
    {
        public static async Task<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken) // where T : BaseClass
        {
            return await Task.Run(() => (T)Deserialize(data, typeof(T)), cancellationToken);
        }

        public static async Task<object> DeserializeAsync(byte[] data, Type type, CancellationToken cancellationToken) // where T : BaseClass
        {
            return await Task.Run(() => Deserialize(data, type), cancellationToken);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(data, typeof(T));
        }

        public static object Deserialize(byte[] data, Type type)
        {
            object obj = null;
            try
            {
                var reader = new BinaryBufferReader(data);

                if (VariantReaders.TryGetValue(type, out var variantReader))
                {
                    obj = variantReader(reader);
                    if (reader.Position != data.Length && type != CommonTypes.TypeOfAbi)
                    {
                        Log.Error($"[{type.Name}] : reader has not read until end {reader.Position} of {data.Length} obj: {obj}");
                    }
                    return obj;
                }
                else
                {
                    obj = GetTypeReader(type)(reader, type);
                    if (reader.Position != data.Length && type != CommonTypes.TypeOfAbi)
                        Log.Error($"[{type.Name}] : reader has not read until end {reader.Position} of {data.Length} obj: {obj}");
                    return obj;
                }
            }
            catch (EndOfStreamException)
            {
                Log.Error($"[{type.Name}] End of stream ", "");
                if(obj != null)
                    Log.Information($"obj: {obj}");
            }
            catch (Exception e)
            {
                Log.Error(e,"");
            }
            return Activator.CreateInstance(type);
        }

        public static object ReadReferenceType(BinaryBufferReader binaryReader, Type type)
        {
            var obj = Activator.CreateInstance(type);
            var objRef = __makeref(obj);

            /*var fields = type.GetFields().All(f => Attribute.IsDefined(f, CommonTypes.TypeOfSortOrderAttribute))
                ? type.GetFields().OrderBy(f =>
                    // ReSharper disable once PossibleNullReferenceException
                    ((SortOrderAttribute) f.GetCustomAttribute(CommonTypes.TypeOfSortOrderAttribute, true)).Order).ToList()
                : type.GetFields().ToList();*/

            foreach (var fieldInfo in TryCachedFieldInfos(type))
            {
                var fieldType = fieldInfo.Key.FieldType;

                if (VariantReaders.TryGetValue(fieldType, out var variantReader))
                {
                    var value = fieldInfo.Value
                        ? ReadOptional(binaryReader, variantReader)
                        : variantReader(binaryReader);
                    if(value != null)
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

                    if(value != null)
                        fieldInfo.Key.SetValueDirect(objRef, value);
                }
            }

            return obj;
        }

        private static readonly ConcurrentDictionary<Type, List<KeyValuePair<FieldInfo, bool>>> CachedFieldInfos = new();

        private static List<KeyValuePair<FieldInfo, bool>> TryCachedFieldInfos(Type type)
        {
            if (CachedFieldInfos.TryGetValue(type, out var fields))
                return fields;

            fields = type.GetFields().All(f => Attribute.IsDefined(f, CommonTypes.TypeOfSortOrderAttribute))
                ? type.GetFields().OrderBy(f =>
                    // ReSharper disable once PossibleNullReferenceException
                    ((SortOrderAttribute)f.GetCustomAttribute(CommonTypes.TypeOfSortOrderAttribute, true)).Order).Select(f => new KeyValuePair<FieldInfo, bool>(f, NullableHelper.IsNullable(f))).ToList()
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
            /*else if (fieldType.IsValueType && !fieldType.IsEnum && !fieldType.IsPrimitive)
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
            else if (fieldType.IsArray)  // array types
            {
                typeReader = ReadArray;
            }
            else if (fieldType.IsGenericType)    // generic types IList<>, IDictionary etc.
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
            return (int)binaryReader.ReadByte();
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

        private static object ReadArray(BinaryBufferReader binaryReader, Type fieldType)
        {
            var elementType = fieldType.GetElementType();
            var size = Convert.ToInt32(binaryReader.ReadVarUint32());

            var items = (Array)Activator.CreateInstance(fieldType, new object[] { size });
            if (size > 0)
            {
                if (VariantReaders.TryGetValue(elementType!, out var variantReader))
                {
                    for (var i = 0; i < size; i++)
                    {
                        items?.SetValue(variantReader(binaryReader), i);
                    }
                }
                else
                {

                    var typeReader = GetTypeReader(elementType);

                    for (var i = 0; i < size; i++)
                    {
                        items?.SetValue(typeReader(binaryReader, elementType), i);
                    }
                }
            }

            return items;
        }

        private static object ReadGeneric(BinaryBufferReader binaryReader, Type fieldType)
        {
            var genericType = fieldType.GetGenericTypeDefinition();
            var genericArgs = fieldType.GetGenericArguments();
            var size = Convert.ToInt32(binaryReader.ReadVarUint32());

            if (VariantReaders.TryGetValue(fieldType, out var genTypeReader))
            {
                genTypeReader(binaryReader);
            }
            else if (genericType == typeof(IDictionary))
            {
                var items = (IDictionary)Activator.CreateInstance(fieldType);
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
            }
            else if (genericType == typeof(IList))
            {
                var items = (IList)Activator.CreateInstance(fieldType);
                if (genericArgs.Length == 1)
                {
                    var typeReader = GetTypeReader(genericArgs[0]);
                    for (var i = 0; i < size; i++)
                    {
                        items?.Add(typeReader(binaryReader, genericArgs[0]));
                    }
                }
            }
            else
            {
                Log.Information($"Generic Type {fieldType.Name} not allowed");
            }
            return null;
        }

        private static readonly Dictionary<Type, ReaderDelegate> PrimitiveReaders = new()
        {
            { CommonTypes.TypeOfString, (reader, _) => reader.ReadEosString() },  // We know, string is not a primitive... we treat it like that anyway.
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
            { CommonTypes.TypeOfVarUint32, (reader, _) => reader.ReadVarUint32() },
            { CommonTypes.TypeOfVarInt32, (reader, _) => reader.ReadVarInt32() },
            { CommonTypes.TypeOfVarUint64, (reader, _) => reader.ReadVarUint64() },
            { CommonTypes.TypeOfVarInt64, (reader, _) => reader.ReadVarInt64() },
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
            //{ CommonTypes.TypeOfDeltasBytes, (reader, _) => reader.ReadDeltasBytes() },
            //{ CommonTypes.TypeOfBlockBytes, (reader, _) => reader.ReadBlockBytes() },
            //{ CommonTypes.TypeOfContractRowBytes, (reader, _) => reader.ReadContractRowBytes() },
            //{ CommonTypes.TypeOfActionBytes, (reader, _) => reader.ReadActionBytes() },
            //{ CommonTypes.TypeOfPackedTransactionBytes, (reader, _) => reader.ReadPackedTransactionBytes() },
            { CommonTypes.TypeOfAsset, (reader, _) => reader.ReadAsset() },
            { CommonTypes.TypeOfSymbol, (reader, _) => reader.ReadSymbol() },
            { CommonTypes.TypeOfSymbolCode, (reader, _) => reader.ReadSymbolCode() },
            { CommonTypes.TypeOfTimestamp, (reader, _) => reader.ReadTimestamp() },
            //            {CommonTypes.TypeOfRowData, (reader, _) => reader.ReadRowData()}
        };

        private static readonly Dictionary<Type, VariantReaderDelegate> VariantReaders = new()
        {
            { CommonTypes.TypeOfBlockSigningAuthorityVariant, ReadBlockSigningAuthorityVariant },
            { CommonTypes.TypeOfTransactionVariant, ReadTransactionVariant },
            //{ CommonTypes.TypeOfTransactionVariant, ReadTransactionVariant },
            //{ CommonTypes.TypeOfTransactionTraceVariant, ReadTransactionTraceVariant },
            //{ CommonTypes.TypeOfTableDeltaVariant, ReadTableDeltaVariant },
            //{ CommonTypes.TypeOfActionTraceVariant, ReadActionTraceVariant },
            //{ CommonTypes.TypeOfPartialTransactionVariant, ReadPartialTransactionVariant },
            //{ CommonTypes.TypeOfActionReceiptVariant, ReadActionReceiptVariant },
            //{ CommonTypes.TypeOfRequestVariant, ReadRequestVariant },
            //{ CommonTypes.TypeOfResultVariant, ReadResultVariant },
            //{ CommonTypes.TypeOfAccountVariant, ReadAccountVariant },
            //{ CommonTypes.TypeOfAccountMetadataVariant, ReadAccountMetadataVariant },
            //{ CommonTypes.TypeOfCodeVariant, ReadCodeVariant },
            //{ CommonTypes.TypeOfContractTableVariant, ReadContractTableVariant },
            //{ CommonTypes.TypeOfContractRowVariant, ReadContractRowVariant },
            //{ CommonTypes.TypeOfContractIndex64Variant, ReadContractIndex64Variant },
            //{ CommonTypes.TypeOfContractIndex128Variant, ReadContractIndex128Variant },
            //{ CommonTypes.TypeOfContractIndex256Variant, ReadContractIndex256Variant },
            //{ CommonTypes.TypeOfContractIndexDoubleVariant, ReadContractIndexDoubleVariant },
            //{ CommonTypes.TypeOfContractIndexLongDoubleVariant, ReadContractIndexLongDoubleVariant },
            //{ CommonTypes.TypeOfChainConfigVariant, ReadChainConfigVariant },
            //{ CommonTypes.TypeOfGlobalPropertyVariant, ReadGlobalPropertyVariant },
            //{ CommonTypes.TypeOfGeneratedTransactionVariant, ReadGeneratedTransactionVariant },
            //{ CommonTypes.TypeOfActivatedProtocolFeatureVariant, ReadActivatedProtocolFeatureVariant },
            //{ CommonTypes.TypeOfProtocolStateVariant, ReadProtocolStateVariant },
            //{ CommonTypes.TypeOfPermissionVariant, ReadPermissionVariant },
            //{ CommonTypes.TypeOfPermissionLinkVariant, ReadPermissionLinkVariant },
            //{ CommonTypes.TypeOfResourceLimitsVariant, ReadResourceLimitsVariant },
            //{ CommonTypes.TypeOfUsageAccumulatorVariant, ReadUsageAccumulatorVariant },
            //{ CommonTypes.TypeOfResourceUsageVariant, ReadResourceUsageVariant },
            //{ CommonTypes.TypeOfResourceLimitsStateVariant, ReadResourceLimitsStateVariant },
            //{ CommonTypes.TypeOfResourceLimitsRatioVariant, ReadResourceLimitsRatioVariant },
            //{ CommonTypes.TypeOfElasticLimitParametersVariant, ReadElasticLimitParametersVariant },
            //{ CommonTypes.TypeOfResourceLimitsConfigVariant, ReadResourceLimitsConfigVariant },
            //{ CommonTypes.TypeOfBlockSigningAuthorityVariant, ReadBlockSigningAuthorityVariant },
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

        

        //public static TransactionVariant ReadTransactionVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    return i switch
        //    {
        //        0 => reader.ReadChecksum256(),
        //        1 => (PackedTransactionVariant)ReadReferenceType(reader, CommonTypes.TypeOfPackedTransaction),
        //        _ => throw new Exception($"TransactionVariant VariantType {i} unknown")
        //    };
        //}

        //public static TransactionTrace ReadTransactionTraceVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (TransactionTraceV0)ReadReferenceType(reader, CommonTypes.TypeOfTransactionTraceV0);
        //    throw new Exception("TransactionTrace VariantType unknown");
        //}

        //public static TableDelta ReadTableDeltaVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (TableDeltaV0)ReadReferenceType(reader, CommonTypes.TypeOfTableDeltaV0);
        //    throw new Exception("TableDelta VariantType unknown");
        //}

        //public static ActionTrace ReadActionTraceVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ActionTraceV0)ReadReferenceType(reader, CommonTypes.TypeOfActionTraceV0);
        //    throw new Exception("ActionTrace VariantType unknown");
        //}

        //public static PartialTransaction ReadPartialTransactionVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (PartialTransactionV0)ReadReferenceType(reader, CommonTypes.TypeOfPartialTransactionV0);
        //    throw new Exception("PartialTransaction VariantType unknown");
        //}

        //public static ActionReceipt ReadActionReceiptVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ActionReceiptV0)ReadReferenceType(reader, CommonTypes.TypeOfActionReceiptV0);
        //    throw new Exception("ActionReceipt VariantType unknown");
        //}

        //public static Request ReadRequestVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    return i switch
        //    {
        //        0 => (GetStatusRequestV0)ReadReferenceType(reader, CommonTypes.TypeOfGetStatusRequestV0),
        //        1 => (GetBlocksRequestV0)ReadReferenceType(reader, CommonTypes.TypeOfGetBlocksRequestV0),
        //        2 => (GetBlocksAckRequestV0)ReadReferenceType(reader, CommonTypes.TypeOfGetBlocksAckRequestV0),
        //        _ => throw new Exception("Request VariantType unknown")
        //    };
        //}

        //public static Result ReadResultVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    return i switch
        //    {
        //        0 => (GetStatusResultV0)ReadReferenceType(reader, CommonTypes.TypeOfGetStatusResultV0),
        //        1 => (GetBlocksResultV0)ReadReferenceType(reader, CommonTypes.TypeOfGetBlocksResultV0),
        //        _ => throw new Exception("Result VariantType unknown")
        //    };
        //}

        //public static Account ReadAccountVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (AccountV0)ReadReferenceType(reader, CommonTypes.TypeOfAccountV0);
        //    throw new Exception("Account VariantType unknown");
        //}

        //public static AccountMetadata ReadAccountMetadataVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (AccountMetadataV0)ReadReferenceType(reader, CommonTypes.TypeOfAccountMetadataV0);
        //    throw new Exception("AccountMetadata VariantType unknown");
        //}

        //public static Code ReadCodeVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (CodeV0)ReadReferenceType(reader, CommonTypes.TypeOfCodeV0);
        //    throw new Exception("Code VariantType unknown");
        //}

        //public static ContractTable ReadContractTableVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractTableV0)ReadReferenceType(reader, CommonTypes.TypeOfContractTableV0);
        //    throw new Exception("ContractTable VariantType unknown");
        //}

        //public static ContractRow ReadContractRowVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractRowV0)ReadReferenceType(reader, CommonTypes.TypeOfContractRowV0);
        //    throw new Exception("ContractRow VariantType unknown");
        //}

        //public static ContractIndexDouble ReadContractIndexDoubleVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractIndexDoubleV0)ReadReferenceType(reader, CommonTypes.TypeOfContractIndexDoubleV0);
        //    throw new Exception("ContractIndexDouble VariantType unknown");
        //}

        //public static ChainConfig ReadChainConfigVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ChainConfigV0)ReadReferenceType(reader, CommonTypes.TypeOfChainConfigV0);
        //    throw new Exception("ChainConfig VariantType unknown");
        //}

        //public static GlobalProperty ReadGlobalPropertyVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    return i switch
        //    {
        //        0 => (GlobalPropertyV0)ReadReferenceType(reader, CommonTypes.TypeOfGlobalPropertyV0),
        //        1 => (GlobalPropertyV1)ReadReferenceType(reader, CommonTypes.TypeOfGlobalPropertyV1),
        //        _ => throw new Exception("GlobalProperty VariantType unknown")
        //    };
        //}

        //public static ActivatedProtocolFeature ReadActivatedProtocolFeatureVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ActivatedProtocolFeatureV0)ReadReferenceType(reader, CommonTypes.TypeOfActivatedProtocolFeatureV0);
        //    throw new Exception("ActivatedProtocolFeature VariantType unknown");
        //}

        //public static ProtocolState ReadProtocolStateVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ProtocolStateV0)ReadReferenceType(reader, CommonTypes.TypeOfProtocolStateV0);
        //    throw new Exception("ProtocolState VariantType unknown");
        //}

        //public static Permission ReadPermissionVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (PermissionV0)ReadReferenceType(reader, CommonTypes.TypeOfPermissionV0);
        //    throw new Exception("Permission VariantType unknown");
        //}

        //public static PermissionLink ReadPermissionLinkVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (PermissionLinkV0)ReadReferenceType(reader, CommonTypes.TypeOfPermissionLinkV0);
        //    throw new Exception("PermissionLink VariantType unknown");
        //}

        //public static GeneratedTransaction ReadGeneratedTransactionVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (GeneratedTransactionV0)ReadReferenceType(reader, CommonTypes.TypeOfGeneratedTransactionV0);
        //    throw new Exception("GeneratedTransaction VariantType unknown");
        //}

        //public static ContractIndex64 ReadContractIndex64Variant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractIndex64V0)ReadReferenceType(reader, CommonTypes.TypeOfContractIndex64V0);
        //    throw new Exception("ContractIndex64 VariantType unknown");
        //}

        //public static ContractIndex256 ReadContractIndex256Variant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractIndex256V0)ReadReferenceType(reader, CommonTypes.TypeOfContractIndex256V0);
        //    throw new Exception("ContractIndex256 VariantType unknown");
        //}

        //public static ContractIndex128 ReadContractIndex128Variant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractIndex128V0)ReadReferenceType(reader, CommonTypes.TypeOfContractIndex128V0);
        //    throw new Exception("ContractIndex128 VariantType unknown");
        //}

        //public static ContractIndexLongDouble ReadContractIndexLongDoubleVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ContractIndexLongDoubleV0)ReadReferenceType(reader, CommonTypes.TypeOfContractIndexLongDoubleV0);
        //    throw new Exception("ContractIndexLongDouble VariantType unknown");
        //}

        //public static object ReadRowData(byte[] data, string rowType)
        //{
        //    switch (rowType)
        //    {
        //        case "resource_usage":
        //            return (ResourceUsage)Deserialize(data, CommonTypes.TypeOfResourceUsageVariant);
        //        case "resource_limits_state":
        //            return (ResourceLimitsState)Deserialize(data, CommonTypes.TypeOfResourceLimitsStateVariant);
        //        case "resource_limits_config":
        //            return (ResourceLimitsConfig)Deserialize(data, CommonTypes.TypeOfResourceLimitsConfigVariant);
        //        case "account":
        //            return (Account)Deserialize(data, CommonTypes.TypeOfAccountVariant);
        //        case "account_metadata":
        //            return (AccountMetadata)Deserialize(data, CommonTypes.TypeOfAccountMetadataVariant);
        //        case "code":
        //            return (Code)Deserialize(data, CommonTypes.TypeOfCodeVariant);
        //        case "contract_table":
        //            return (ContractTable)Deserialize(data, CommonTypes.TypeOfContractTableVariant);
        //        case "contract_row":
        //            return (ContractRow)Deserialize(data, CommonTypes.TypeOfContractRowVariant);
        //        case "permission":
        //            return (Permission)Deserialize(data, CommonTypes.TypeOfPermissionVariant);
        //        case "permission_link":
        //            return (PermissionLink)Deserialize(data, CommonTypes.TypeOfPermissionLinkVariant);
        //        case "resource_limits":
        //            return (ResourceLimits)Deserialize(data, CommonTypes.TypeOfResourceLimitsVariant);
        //        case "global_property":
        //            return (GlobalProperty)Deserialize(data, CommonTypes.TypeOfGlobalPropertyVariant);
        //        case "contract_index64":
        //            return (ContractIndex64)Deserialize(data, CommonTypes.TypeOfContractIndex64Variant);
        //        case "contract_index128":
        //            return (ContractIndex128)Deserialize(data, CommonTypes.TypeOfContractIndex128Variant);
        //        case "contract_index256":
        //            return (ContractIndex256)Deserialize(data, CommonTypes.TypeOfContractIndex256Variant);
        //        case "contract_index_double":
        //            return (ContractIndexDouble)Deserialize(data, CommonTypes.TypeOfContractIndexDoubleVariant);
        //        case "contract_index_long_double":
        //            return (ContractIndexLongDouble)Deserialize(data, CommonTypes.TypeOfContractIndexLongDoubleVariant);
        //        case "generated_transaction":
        //            return (GeneratedTransaction)Deserialize(data, CommonTypes.TypeOfGeneratedTransactionVariant);
        //        case "protocol_state":
        //            return (ProtocolState)Deserialize(data, CommonTypes.TypeOfProtocolStateVariant);
        //        default:
        //            throw new Exception($"RowType {rowType.ToString()} unknown");
        //    }
        //}


        //public static ResourceLimits ReadResourceLimitsVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ResourceLimitsV0)ReadReferenceType(reader, CommonTypes.TypeOfResourceLimitsV0);
        //    throw new Exception("ResourceLimits VariantType unknown");
        //}

        //public static UsageAccumulator ReadUsageAccumulatorVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (UsageAccumulatorV0)ReadReferenceType(reader, CommonTypes.TypeOfUsageAccumulatorV0);
        //    throw new Exception("UsageAccumulator VariantType unknown");
        //}

        //public static ResourceUsage ReadResourceUsageVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ResourceUsageV0)ReadReferenceType(reader, CommonTypes.TypeOfResourceUsageV0);
        //    throw new Exception("ResourceUsage VariantType unknown");
        //}

        //public static ResourceLimitsState ReadResourceLimitsStateVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ResourceLimitsStateV0)ReadReferenceType(reader, CommonTypes.TypeOfResourceLimitsStateV0);
        //    throw new Exception("ResourceLimitsState VariantType unknown");
        //}

        //public static ResourceLimitsRatio ReadResourceLimitsRatioVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ResourceLimitsRatioV0)ReadReferenceType(reader, CommonTypes.TypeOfResourceLimitsRatioV0);
        //    throw new Exception("ResourceLimitsRatio VariantType unknown");
        //}

        //public static ElasticLimitParameters ReadElasticLimitParametersVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ElasticLimitParametersV0)ReadReferenceType(reader, CommonTypes.TypeOfElasticLimitParametersV0);
        //    throw new Exception("ElasticLimitParameters VariantType unknown");
        //}

        //public static ResourceLimitsConfig ReadResourceLimitsConfigVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (ResourceLimitsConfigV0)ReadReferenceType(reader, CommonTypes.TypeOfResourceLimitsConfigV0);
        //    throw new Exception("ResourceLimitsConfig VariantType unknown");
        //}

        //public static BlockSigningAuthority ReadBlockSigningAuthorityVariant(BinaryBufferReader reader)
        //{
        //    var i = Convert.ToInt32(reader.ReadVarUint32());
        //    if (i == 0)
        //        return (BlockSigningAuthorityV0)ReadReferenceType(reader, CommonTypes.TypeOfBlockSigningAuthorityV0);
        //    throw new Exception("BlockSigningAuthority VariantType unknown");
        //}

        #endregion Variants
    }
}
