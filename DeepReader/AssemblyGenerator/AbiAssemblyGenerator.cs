using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using DeepReader.Types.EosTypes;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc;
using Serilog;
using System.Text.RegularExpressions;
using Action = DeepReader.Types.Eosio.Chain.Action;

namespace DeepReader.AssemblyGenerator
{
    internal class AbiAssemblyGenerator
    {
        #region 

        private static readonly Type BinaryReaderType = typeof(BinaryReader);

        private static readonly Type[] binaryReaderAttr = new Type[] { BinaryReaderType };

        #endregion

        private readonly Abi _abi;
        private readonly Name _contractName;
        private readonly ulong _globalSequence;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly List<AbiStruct> _abiStructsWithBaseFields = new List<AbiStruct>();


        /*                      ABI's
         * 
         * version              : version specifying the abi-format, not the version of the actuall abi
         * types                : (name->type pairs) typedefs to give structs a new/different name
         * structs              : actual structs used for actions, tables, etc.
         * actions              : (name->type pairs) pointing to the struct of the action
         * tables               : (name->type pairs + additional info) pointing to the struct of the table
         * ricardian_clauses    : ...
         * error_messages       : ...
         * abi_extensions       : ...
         * variants             : TODO
         */

        public AbiAssemblyGenerator(Abi abi, Name contractName, ulong globalSequence)
        {
            _abi = abi;
            _contractName = contractName;
            _globalSequence = globalSequence;

            _abiStructsWithBaseFields = AbiStructsWithBaseFields(abi);

            var assemblyName = new AssemblyName(_contractName.StringVal.Replace('.','_'));
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule("Main");

            Log.Information($"Abi for {_contractName.StringVal}");
        }

        public bool TryGenerateAssembly(out Assembly? assembly)
        {
            try
            {
                foreach (var abiStructWithBaseFields in _abiStructsWithBaseFields)
                {
                    if (TryVerifyTypeNameAndGetClrTypeName(abiStructWithBaseFields.Name, out var clrTypeName))
                    {
                        var abiClrFieldType = _moduleBuilder.GetTypes().FirstOrDefault(t => t.FullName == clrTypeName);
                        // Create new ClrType if module does not already contain it
                        // creation of a ClrType can recursively create other ClrTypes so it's possible types are added
                        // to the module before the loop reaches them
                        if (abiClrFieldType == null)
                            CreateAbiStructClrType(abiStructWithBaseFields.Name);
                    }
                }

#if DEBUG
                SaveAssemblyAndAbi(_moduleBuilder.Assembly, _abi, _contractName, _globalSequence);
#endif

                assembly = _moduleBuilder.Assembly;
                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "");
            }
            assembly = null;
            return false;
        }

#if DEBUG
//        private static readonly string AssemblyPath = "/mnt/e/source/repos/deepreader/DeepReader/config/mindreader/abis/";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAssemblies");
        private static readonly string AssemblyPath = "/app/config/mindreader/abis/";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAssemblies");

        private static async void SaveAssemblyAndAbi(Assembly assembly, Abi abi, string contractName, ulong globalSequence)
        {
            try
            {
                var generator = new Lokad.ILPack.AssemblyGenerator();

                var path = Path.Combine(AssemblyPath, contractName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                await using (var fileStream = File.CreateText(Path.Combine(path, contractName + "." + globalSequence + ".abi")).BaseStream)
                {
                    await JsonSerializer.SerializeAsync(fileStream, abi, typeof(Abi), new JsonSerializerOptions()
                    {
                        IncludeFields = true,
                        MaxDepth = Int32.MaxValue,
                        WriteIndented = true
                    });
                }
                generator.GenerateAssembly(assembly, Path.Combine(path, contractName + "." + globalSequence + ".dll"));
            }
            catch(Exception ex)
            {
                Log.Error(ex, "");
            }
        }
#endif

        /// <summary>
        /// returns a new List of AbiStructs with all base-fields (if any)
        /// also renames typefed types to their original type-names
        /// </summary>
        /// <param name="abi"></param>
        /// <returns>List of AbiStructs containing all base-fields </returns>
        private List<AbiStruct> AbiStructsWithBaseFields(Abi abi)
        {
            var typedefDict = abi.AbiTypes.ToDictionary(k => k.NewTypeName, v => v.Type);
            var abiStructsWithBaseFields = new List<AbiStruct>();
            foreach (var abiStruct in abi.AbiStructs)
            {
                var abiStructWithBaseFields = GetAbiStructWithBaseFields(abiStruct.Name, abiStruct);

                // Resetting of fieldtype-names to original (un-typedefed) struct-names
                foreach (var abiField in abiStructWithBaseFields.Fields)
                {
                    if (typedefDict.ContainsKey(abiField.Type))
                        abiField.Type = typedefDict[abiField.Type];
                }

                abiStructsWithBaseFields.Add(abiStructWithBaseFields);
            }

            return abiStructsWithBaseFields;
        }

        /// <summary>
        /// recursively goes through base-structs and adds base-struct fields to the returned AbiStruct
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="abiStruct"></param>
        /// <returns> AbiStruct containing AbiFields of parent and all base-structs</returns>
        private AbiStruct GetAbiStructWithBaseFields(string abiStructName, AbiStruct? abiStruct = null)
        {
            var abiStructWithBaseFields = _abiStructsWithBaseFields.FirstOrDefault(a => a.Name == abiStructName);
            // if we already processed this struct, return it
            if(abiStructWithBaseFields != null)
                return abiStructWithBaseFields;

            // add the fields of the initial struct to the list
            var abiFields = new List<AbiField>();
            if(abiStruct != null)
                abiFields.AddRange(abiStruct.Fields);

            if (!string.IsNullOrWhiteSpace(abiStruct?.Base))
            {
                var baseStruct = GetAbiStructWithBaseFields(abiStruct.Base);
                abiFields.AddRange(baseStruct.Fields);
            }

            return new AbiStruct(abiStructName, abiFields);
        }


        /// <summary>
        /// Searches for the CLR-Type of the AbiField in the current module
        /// Creates the CLR-Type if it's not found
        /// returns the CLR-Type and wether it's optional/nullable and/or an array
        /// </summary>
        /// <param name="abiField"></param>
        /// <returns></returns>
        private (Type? abiClrFieldType, bool isOptional, bool isArrayType, bool isModuleType) GetClrAbiFieldTypeInfo(AbiField abiField)
        {
            string fieldTypeName = abiField.Type;
            bool isOptional = false;
            bool isArrayType = false;
            bool isModuleType = true; // if this type a Type contained in this Assembly
            if (fieldTypeName.EndsWith("[]"))
            {
                fieldTypeName = abiField.Type.Replace("[]", "");
                isArrayType = true;
            }
            else if (fieldTypeName.EndsWith("$")) // TODO
            {
                fieldTypeName = abiField.Type.Replace("$", "");
            }
            else if (fieldTypeName.EndsWith("?"))
            {
                fieldTypeName = abiField.Type.Replace("?", "");
                isOptional = true;
            }

            if(TryVerifyTypeNameAndGetClrTypeName(fieldTypeName, out var clrTypeName))
            {
                if (TypeMap.TryGetValue(fieldTypeName, out var abiClrFieldType))
                    isModuleType = false; // if this type was found in the TypeMap it's not a moduleType, in all other cases it is
                else
                    abiClrFieldType = _moduleBuilder.GetTypes().FirstOrDefault(t => t.FullName == clrTypeName);

                if (abiClrFieldType == null)
                    abiClrFieldType = CreateAbiStructClrType(fieldTypeName);

                return (abiClrFieldType, isOptional, isArrayType, isModuleType);
            }
            return (null, isOptional, isArrayType, isModuleType);
        }

        private Type? CreateAbiStructClrType(string abiStructName)
        {
            try
            {
                var abiStruct = _abiStructsWithBaseFields.FirstOrDefault(a => a.Name == abiStructName);
                if (abiStruct == null)
                {
                    Log.Error( new Exception($"abiStruct {abiStructName} not found in abiStructsList"),"");
                    return null;
                }

                // Guess we should check that a type-name/class name is not longer than 511 characters + some other checks
                // https://stackoverflow.com/questions/186523/what-is-the-maximum-length-of-a-c-cli-identifier
                if (TryVerifyTypeNameAndGetClrTypeName(abiStructName, out var clrTypeName))
                {
                    var dynamicType = _moduleBuilder.DefineType(clrTypeName,
                        TypeAttributes.Public |
                        TypeAttributes.Sealed |
                        TypeAttributes.SequentialLayout,
                        typeof(object));

                    // Constructor
                    var defaultConstructor = dynamicType.DefineDefaultConstructor(MethodAttributes.Public);
                    //var defaultConstructorIlGenerator = defaultConstructor.GetILGenerator();
                    var binaryReaderConstructor = dynamicType.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis | CallingConventions.Standard, binaryReaderAttr);
                    var binaryReaderConstructorIlGenerator = binaryReaderConstructor.GetILGenerator();

                    // Load "this"
                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);

                    // Get the constructor of our new type with BinaryReader-Param so we can add generated IL-Code
                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, defaultConstructor); // TODO, call Object-constructor instead?

                    // Iterate all Fields, writes IL-Code to set their values
                    foreach (var abiField in abiStruct.Fields)
                    {
                        if (TryVerifyFieldNameAndGetClrFieldName(abiField.Name, out string clrFieldName))
                        {
                            var (abiFieldType, isOptional, isArrayType, isModuleType) = GetClrAbiFieldTypeInfo(abiField);

                            if (abiFieldType == null)
                            {
                                Log.Error(new Exception($"unable to get or generate type for {abiField.Type}"), "");
                                continue;
                            }

                            // Array-specific IL-Code
                            if (isArrayType)
                            {
                                // make the type an array
                                var abiArrayFieldType = abiFieldType.MakeArrayType();

                                var fieldBuilder = dynamicType.DefineField(clrFieldName, abiArrayFieldType, FieldAttributes.Public);

                                // i 
                                var iLocal = binaryReaderConstructorIlGenerator.DeclareLocal(typeof(int));
                                // loop condition local variable
                                var condLocal = binaryReaderConstructorIlGenerator.DeclareLocal(typeof(bool));

                                // label at loop start/begin, loop jumps back here (where it's marked)
                                var loopStartLabel = binaryReaderConstructorIlGenerator.DefineLabel();
                                // loop body label, first iteration jumps here (where it's marked)
                                var loopBodyLabel = binaryReaderConstructorIlGenerator.DefineLabel();

                                // get the readerMethod for this type
                                var readerMethod = GetReaderMethodForType(abiFieldType, abiField);

                                // Load "this" onto stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);

                                // Load BinaryReader reference onto stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1);

                                // Read the length of the array and put it onto the stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Callvirt, Read7BitEncodedInt);

                                // Call the constructor for the array, length from stack is used to initialize size
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Newarr, abiFieldType);

                                // set array to new array
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);

                                // put value 0 as int32 onto the stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_0);

                                // pop 0 from stack into local list at index 0
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_0);

                                // start loop
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Br_S, loopBodyLabel);

                                // loop jumps back here
                                binaryReaderConstructorIlGenerator.MarkLabel(loopStartLabel);

                                // this
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);

                                // Set value at current array-index
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);

                                // i
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_0, iLocal);

                                // reader
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1);

                                // Call the Reader/Deserialization-Method
                                // if this type is part of the generated module/assembly, call it's the constructor
                                // if not, call it's deserialization-method
                                if (isModuleType)
                                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, abiFieldType.GetConstructor(binaryReaderAttr)!);
                                else
                                {
                                    var meth = GetReaderMethodForType(abiFieldType, abiField);
                                    if (meth.IsVirtual)
                                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Callvirt, meth);
                                    else
                                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, meth);
                                }

                                // set array field
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Stelem_Ref);

                                // load i
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_0, iLocal);

                                // push 1 onto stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_1);

                                // add 1 to i
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Add);

                                // pop i
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_0, iLocal);

                                // loop initially jumps here, loop condition follows
                                binaryReaderConstructorIlGenerator.MarkLabel(loopBodyLabel);

                                // load i
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_0, iLocal);

                                // this
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);

                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);

                                // push array length onto stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldlen);

                                // convert to int32
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Conv_I4);

                                // compare i and length
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Clt);

                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_1, condLocal);

                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_1, condLocal);

                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Brtrue_S, loopStartLabel);

                            }
                            else
                            {
                                var fieldBuilder = dynamicType.DefineField(clrFieldName, abiFieldType, FieldAttributes.Public);

                                Label readOptionalEnd = default;
                                // if the Field is optional we need to make it Nullable
                                // and we need to generate code that handle's it's deserialization with a conditional
                                if (isOptional)
                                {
                                    // define label to jump to in case condition is false
                                    readOptionalEnd = binaryReaderConstructorIlGenerator.DefineLabel();
                                    // Load "this" onto stack
                                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);
                                    // Load BinaryReader reference onto stack
                                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1);
                                    // Read Byte/Boolean (put on stack)
                                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, ReadBoolean);
                                    // check value on stack, if false, skip and continue at readOptionalEnd-Label
                                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Brfalse_S, readOptionalEnd);

                                    // if this is a ValueType we make it nullable
                                    // if not a ValueType we need to set a Custom NullableAttribute when adding it as Field to a type
                                    if (abiFieldType.IsValueType)
                                        abiFieldType = typeof(Nullable<>).MakeGenericType(abiFieldType);
                                    else
                                        fieldBuilder.SetCustomAttribute(NullableHelper.NullableAttributeBuilder);
                                }

                                // Load "this" onto stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);
                                // Load BinaryReader reference onto stack
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1);

                                // Call the Reader/Deserialization-Method
                                // if this type is part of the generated module/assembly, call it's the constructor
                                // if not, call it's deserialization-method
                                if (isModuleType)
                                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, abiFieldType.GetConstructor(binaryReaderAttr)!);
                                else
                                {
                                    var meth = GetReaderMethodForType(abiFieldType, abiField);
                                    if (meth.IsVirtual)
                                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Callvirt, meth);
                                    else
                                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, meth);
                                }


                                // Set value of field to return-value of preious Call (value was put on stack)
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);

                                if (isOptional) // mark the label (set it's position) so code can jump here
                                    binaryReaderConstructorIlGenerator.MarkLabel(readOptionalEnd);
                            }
                        }
                    }
                    // End of method - return
                    binaryReaderConstructorIlGenerator.Emit(OpCodes.Ret);

                    return dynamicType.CreateType()!;
                }
            }
            catch (Exception e)
            {
                Log.Error(e,"");
                Log.Information($"Exception occured while creating type for {abiStructName}");
            }

            return null;
        }

        private static readonly Regex Validator = new Regex(@"[A-Za-z_][A-Za-z0-9_]*");

        private bool TryVerifyTypeNameAndGetClrTypeName(string typeName, out string clrTypeName)
        {
            if (Validator.IsMatch(typeName))
            {
                clrTypeName = $"_{typeName}"; // we add an extra underscore here because of security, no standard types begin with _
                return true;
            }
            clrTypeName = "";
            return false;
        }


        private bool TryVerifyFieldNameAndGetClrFieldName(string fieldName, out string clrFieldName)
        {
            if (Validator.IsMatch(fieldName))
            {
                clrFieldName = $"_{fieldName}"; // we add an extra underscore here, in case of special keywords
                return true;
            }
            clrFieldName = "";
            return false;
        }

        private MethodInfo GetReaderMethodForType(Type type, AbiField abiField)
        {
            if (!TypeReaderMethodMap.TryGetValue(type, out var methodInfo))
                Log.Information($"ReaderMethod for {type.FullName} {abiField.Name} not found");
            return methodInfo;
        }

        #region BinaryReader standard methods

        private static readonly MethodInfo Read7BitEncodedInt = typeof(BinaryReader).GetMethod(nameof(BinaryReader.Read7BitEncodedInt))!;

        private static MethodInfo Read7BitEncodedInt64 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.Read7BitEncodedInt64))!;

        private static readonly MethodInfo ReadBoolean = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBoolean))!;

        private static readonly MethodInfo ReadByte = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadByte))!;

//        private static MethodInfo ReadBytes = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes))!;// needs arg

        private static readonly MethodInfo ReadChar = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadChar))!;

        private static MethodInfo ReadChars = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadChars))!;// needs arg

        private static readonly MethodInfo ReadDecimal = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadDecimal))!;

        private static readonly MethodInfo ReadFloat = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadSingle))!;

        private static readonly MethodInfo ReadDouble = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadDouble))!;

        private static MethodInfo ReadHalf = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadHalf))!;

        private static readonly MethodInfo ReadInt16 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadInt16))!;

        private static readonly MethodInfo ReadInt32 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadInt32))!;

        private static readonly MethodInfo ReadInt64 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadInt64))!;

        private static readonly MethodInfo ReadSByte = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadSByte))!;

        //private static MethodInfo ReadSingle = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadSingle))!;

        private static readonly MethodInfo ReadString = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadString))!;

        private static readonly MethodInfo ReadUInt16 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt16))!;

        private static readonly MethodInfo ReadUInt32 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32))!;

        private static readonly MethodInfo ReadUInt64 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt64))!;

        private static readonly MethodInfo ReadVarBytes = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadBytes))!; // Bytes, ActionDataBytes


        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>()
        {
            { "bool", typeof(bool) },
            { "uint8", typeof(byte) },
            { "int8", typeof(sbyte) },
            { "byte", typeof(byte) },
            { "bytes", typeof(byte[]) },
            { "char", typeof(char) },
            { "decimal", typeof(decimal) },
            { "float", typeof(float) },
            { "double", typeof(double) },
            { "float32", typeof(float) },
            { "float64", typeof(double) },
            { "int16", typeof(Int16) },
            { "int32", typeof(Int32) },
            { "int64", typeof(Int64) },
            { "string", typeof(string) },
            { "uint16", typeof(UInt16) },
            { "uint32", typeof(UInt32) },
            { "uint64", typeof(UInt64) },

            { "name", typeof(Name) },
            { "public_key", typeof(PublicKey) },
            { "asset", typeof(Asset) },
            { "permission_level", typeof(PermissionLevel) },
            { "permission_level_weight", typeof(PermissionLevelWeight) },
            { "key_weight", typeof(KeyWeight) },
            { "wait_weight", typeof(WaitWeight) },
            { "authority", typeof(Authority) },
            { "checksum160", typeof(Checksum160) },
            { "checksum256", typeof(Checksum256) },
            { "checksum512", typeof(Checksum512) },
            { "int128", typeof(Int128) },
            { "uint128", typeof(Uint128) },
            { "time_point_sec", typeof(UInt32) },
            { "block_timestamp_type", typeof(UInt32) },
            { "account_name", typeof(Name) },
            { "producer_key", typeof(ProducerKey) },
            { "varuint32", typeof(VarUint32) },
            { "varuint64", typeof(VarUint64) },
            { "action", typeof(Name) },
            { "extension", typeof(Extension) },
            { "transaction", typeof(Transaction) },
            { "time_point", typeof(UInt32) },
            { "symbol", typeof(Symbol) },
            { "symbol_code", typeof(SymbolCode) },
            { "block_header", typeof(BlockHeader) },
        };

        // setaccountcpu
        // setaccountnet
        // setaccountram ?

        // cpu_weight
        // net_weight
        // ram_bytes

        #endregion

        #region IEosioSerializable Methods

        private static readonly MethodInfo ReadAbi = typeof(Abi).GetMethod(nameof(Abi.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAbiType = typeof(AbiType).GetMethod(nameof(AbiType.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAbiStruct = typeof(AbiStruct).GetMethod(nameof(AbiStruct.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAbiField = typeof(AbiField).GetMethod(nameof(AbiField.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAbiAction = typeof(AbiAction).GetMethod(nameof(AbiAction.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAbiTable = typeof(AbiTable).GetMethod(nameof(AbiTable.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAccountRamDelta = typeof(AccountRamDelta).GetMethod(nameof(AccountRamDelta.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAccountDelta = typeof(AccountDelta).GetMethod(nameof(AccountDelta.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadAction = typeof(Action).GetMethod(nameof(Action.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadActionReceipt = typeof(ActionReceipt).GetMethod(nameof(ActionReceipt.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadActionTrace = typeof(ActionTrace).GetMethod(nameof(ActionTrace.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadBlockHeader = typeof(BlockHeader).GetMethod(nameof(BlockHeader.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadBlockSigningAuthorityVariant = typeof(BlockSigningAuthorityVariant).GetMethod(nameof(BlockSigningAuthorityVariant.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadBlockSigningAuthorityV0 = typeof(BlockSigningAuthorityV0).GetMethod(nameof(BlockSigningAuthorityV0.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadBlockState = typeof(BlockState).GetMethod(nameof(BlockState.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadPairAccountNameBlockNum = typeof(PairAccountNameBlockNum).GetMethod(nameof(PairAccountNameBlockNum.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadScheduleInfo = typeof(ScheduleInfo).GetMethod(nameof(ScheduleInfo.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadIncrementalMerkle = typeof(IncrementalMerkle).GetMethod(nameof(IncrementalMerkle.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadBlockHeaderState = typeof(BlockHeaderState).GetMethod(nameof(BlockHeaderState.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadProducerAuthority = typeof(ProducerAuthority).GetMethod(nameof(ProducerAuthority.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadProducerAuthoritySchedule = typeof(ProducerAuthoritySchedule).GetMethod(nameof(ProducerAuthoritySchedule.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadProducerKey = typeof(ProducerKey).GetMethod(nameof(ProducerKey.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadPackedTransaction = typeof(PackedTransaction).GetMethod(nameof(PackedTransaction.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadPermissionLevel = typeof(PermissionLevel).GetMethod(nameof(PermissionLevel.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadProducerSchedule = typeof(ProducerSchedule).GetMethod(nameof(ProducerSchedule.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadProtocolFeatureActivationSet = typeof(ProtocolFeatureActivationSet).GetMethod(nameof(ProtocolFeatureActivationSet.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadSharedKeyWeight = typeof(SharedKeyWeight).GetMethod(nameof(SharedKeyWeight.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadSignedBlock = typeof(SignedBlock).GetMethod(nameof(SignedBlock.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadSignedBlockHeader = typeof(SignedBlockHeader).GetMethod(nameof(SignedBlockHeader.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadSignedTransaction = typeof(SignedTransaction).GetMethod(nameof(SignedTransaction.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransaction = typeof(Transaction).GetMethod(nameof(Transaction.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransactionHeader = typeof(TransactionHeader).GetMethod(nameof(TransactionHeader.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransactionReceipt = typeof(TransactionReceipt).GetMethod(nameof(TransactionReceipt.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransactionReceiptHeader = typeof(TransactionReceiptHeader).GetMethod(nameof(TransactionReceiptHeader.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransactionTrace = typeof(TransactionTrace).GetMethod(nameof(TransactionTrace.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadExcept = typeof(Except).GetMethod(nameof(Except.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransactionTraceAuthSequence = typeof(TransactionTraceAuthSequence).GetMethod(nameof(TransactionTraceAuthSequence.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadTransactionVariant = typeof(TransactionVariant).GetMethod(nameof(TransactionVariant.ReadFromBinaryReader))!;

        // TODO better names for these regions
        #region EosTypes BinaryReaderExtension Methods 

        private static readonly MethodInfo ReadAsset = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadAsset))!;

        private static readonly MethodInfo ReadChecksum160 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadChecksum160))!;

        private static readonly MethodInfo ReadChecksum256 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadChecksum256))!;

        private static readonly MethodInfo ReadChecksum512 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadChecksum512))!;

        // ExtendedAsset

        private static readonly MethodInfo ReadFloat128 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadFloat128))!;

        private static readonly MethodInfo ReadInt128 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadInt128))!;

        private static readonly MethodInfo ReadName = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadName))!;

        private static readonly MethodInfo ReadPublicKey = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadPublicKey))!;

        private static readonly MethodInfo ReadSymbol = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadSymbol))!;

        private static readonly MethodInfo ReadSymbolCode = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadSymbolCode))!;

        private static readonly MethodInfo ReadTimestamp = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadTimestamp))!;

        private static readonly MethodInfo ReadUInt128 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadUInt128))!;

        private static readonly MethodInfo ReadAuthority = typeof(Authority).GetMethod(nameof(Authority.ReadFromBinaryReader))!;


        private static readonly MethodInfo ReadVarUint32 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadVarUint32))!;

        private static readonly MethodInfo ReadVarUint64 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadVarUint64))!;


        private static readonly MethodInfo ReadKeyWeight = typeof(KeyWeight).GetMethod(nameof(KeyWeight.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadWaitWeight = typeof(WaitWeight).GetMethod(nameof(WaitWeight.ReadFromBinaryReader))!;
        
        private static readonly MethodInfo ReadPermissionLevelWeight = typeof(PermissionLevelWeight).GetMethod(nameof(PermissionLevelWeight.ReadFromBinaryReader))!;

        private static readonly MethodInfo ReadExtension = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadExtension))!;

        #endregion

        // all EosioSerializable-Types
        private static readonly Dictionary<Type, MethodInfo> TypeReaderMethodMap = new Dictionary<Type, MethodInfo>()
        {
            { typeof(bool), ReadBoolean },
            { typeof(byte), ReadByte },
            { typeof(sbyte), ReadSByte },
            { typeof(byte[]), ReadVarBytes },
            { typeof(char), ReadChar },
            { typeof(decimal), ReadDecimal },
            { typeof(Int16), ReadInt16 },
            { typeof(Int32), ReadInt32 },
            { typeof(Int64), ReadInt64 },
            { typeof(string), ReadString },
            { typeof(UInt16), ReadUInt16 },
            { typeof(UInt32), ReadUInt32 },
            { typeof(UInt64), ReadUInt64 },
            { typeof(float), ReadFloat },
            { typeof(double), ReadDouble },

            { typeof(Abi), ReadAbi },
            { typeof(AbiType), ReadAbiType },
            { typeof(AbiStruct), ReadAbiStruct },
            { typeof(AbiField), ReadAbiField },
            { typeof(AbiAction), ReadAbiAction },
            { typeof(AbiTable), ReadAbiTable },
            { typeof(AccountRamDelta), ReadAccountRamDelta },
            { typeof(AccountDelta), ReadAccountDelta },
            { typeof(Action), ReadAction },
            { typeof(ActionReceipt), ReadActionReceipt },
            { typeof(ActionTrace), ReadActionTrace },
            { typeof(BlockHeader), ReadBlockHeader },
            { typeof(BlockSigningAuthorityVariant), ReadBlockSigningAuthorityVariant },
            { typeof(BlockSigningAuthorityV0), ReadBlockSigningAuthorityV0 },
            { typeof(BlockState), ReadBlockState },
            { typeof(PairAccountNameBlockNum), ReadPairAccountNameBlockNum },
            { typeof(ScheduleInfo), ReadScheduleInfo },
            { typeof(IncrementalMerkle), ReadIncrementalMerkle },
            { typeof(BlockHeaderState), ReadBlockHeaderState },
            { typeof(ProducerAuthority), ReadProducerAuthority },
            { typeof(ProducerAuthoritySchedule), ReadProducerAuthoritySchedule },
            { typeof(ProducerKey), ReadProducerKey },
            { typeof(PackedTransaction), ReadPackedTransaction },
            { typeof(PermissionLevel), ReadPermissionLevel },
            { typeof(ProducerSchedule), ReadProducerSchedule },
            { typeof(ProtocolFeatureActivationSet), ReadProtocolFeatureActivationSet },
            { typeof(SharedKeyWeight), ReadSharedKeyWeight },
            { typeof(SignedBlock), ReadSignedBlock },
            { typeof(SignedBlockHeader), ReadSignedBlockHeader },
            { typeof(SignedTransaction), ReadSignedTransaction },
            { typeof(Transaction), ReadTransaction },
            { typeof(TransactionHeader), ReadTransactionHeader },
            { typeof(TransactionReceipt), ReadTransactionReceipt },
            { typeof(TransactionReceiptHeader), ReadTransactionReceiptHeader },
            { typeof(TransactionTrace), ReadTransactionTrace },
            { typeof(Except), ReadExcept },
            { typeof(TransactionTraceAuthSequence), ReadTransactionTraceAuthSequence },
            { typeof(TransactionVariant), ReadTransactionVariant },

            { typeof(Bytes), ReadVarBytes },
            { typeof(ActionDataBytes), ReadVarBytes },
            { typeof(Checksum160), ReadChecksum160 },
            { typeof(Checksum256), ReadChecksum256 },
            { typeof(Checksum512), ReadChecksum512 },
            { typeof(Float128), ReadFloat128 },
            { typeof(Int128), ReadInt128 },
            { typeof(Name), ReadName },
            { typeof(PublicKey), ReadPublicKey },
            { typeof(Timestamp), ReadTimestamp },
            { typeof(Uint128), ReadUInt128 },

            { typeof(Asset), ReadAsset },
            { typeof(Symbol), ReadSymbol },
            { typeof(SymbolCode), ReadSymbolCode },
            { typeof(Authority), ReadAuthority },

            { typeof(VarUint32), ReadVarUint32 },
            { typeof(VarUint64), ReadVarUint64 },

            { typeof(KeyWeight), ReadKeyWeight },
            { typeof(WaitWeight), ReadWaitWeight },
            { typeof(PermissionLevelWeight), ReadPermissionLevelWeight },
            { typeof(Extension), ReadExtension },
        };

        #endregion


        public class NullableHelper
        {
            public static object? Helper;

            private static CustomAttributeData _nullableAttribute;

            public static CustomAttributeData NullableAttribute = _nullableAttribute ?? GetNullableAttribute();

            private static CustomAttributeData GetNullableAttribute()
            {
                _nullableAttribute = typeof(NullableHelper).GetFields()[0].CustomAttributes.FirstOrDefault(x =>
                    x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute")!;
                return _nullableAttribute;
            }

            private static CustomAttributeBuilder _nullableAttributeBuilder;

            public static CustomAttributeBuilder NullableAttributeBuilder =
                _nullableAttributeBuilder ?? GetNullableAttributeBuilder();

            private static CustomAttributeBuilder GetNullableAttributeBuilder()
            {
                _nullableAttribute = typeof(NullableHelper).GetFields()[0].CustomAttributes.FirstOrDefault(x =>
                    x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute")!;

                var nullableAttr = NullableAttribute;
                var constructorAttributes = nullableAttr.ConstructorArguments.Select(m => m.Value).ToArray();
                _nullableAttributeBuilder = new CustomAttributeBuilder(nullableAttr.Constructor, constructorAttributes);
                return _nullableAttributeBuilder;
            }
        }
    }
}
