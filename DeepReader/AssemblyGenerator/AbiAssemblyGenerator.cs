﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using DeepReader.Types.EosTypes;
using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using Action = DeepReader.Types.Eosio.Chain.Action;
using DeepReader.Types.Eosio.Chain.Detail;
using DeepReader.Types.Eosio.Chain.Legacy;
using DeepReader.Types.Extensions;
using DeepReader.Types.Fc;
using Serilog;

namespace DeepReader.AssemblyGenerator
{
    internal class AbiAssemblyGenerator
    {
        // TODO
        // not sure if this is needed or if we should use the base-ABI ?! ... 

        private Abi _abi;
        private Name _contractName;
        private ulong _globalSequence;
        // TODO global_sequence
        private ModuleBuilder _moduleBuilder;
        private List<AbiStruct> _abiStructsWithBaseFields = new List<AbiStruct>();


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
            // TODO global_sequence?

            _abiStructsWithBaseFields = AbiStructsWithBaseFields(abi);

            var assemblyName = new AssemblyName(_contractName.StringVal.Replace('.','_'));
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule("Main");

            Log.Information($"Abi for {_contractName.StringVal}");

        }

        public Assembly GenerateAssembly()
        {
            foreach (var abiStructWithBaseFields in _abiStructsWithBaseFields)
            {
                var clrTypeName = $"{abiStructWithBaseFields.Name}";
                var abiClrFieldType = _moduleBuilder.GetTypes().FirstOrDefault(t => t.FullName == clrTypeName);
                // Create new ClrType if module does not already contain it
                // creation of a ClrType can recursively create other ClrTypes so it's possible types are added
                // to the module before the loop reaches them
                if (abiClrFieldType == null)
                    CreateAbiStructClrType(abiStructWithBaseFields.Name);
            }

#if DEBUG
            SaveAssemblyAndAbi(_moduleBuilder.Assembly, _abi, _contractName, _globalSequence);
#endif

            return _moduleBuilder.Assembly;
        }

        public static string AssemblyPath = "/app/config/mindreader/abis/";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedAssemblies");

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

            //foreach (var abiStructsWithBaseField in abiStructsWithBaseFields)
            //{
            //    Log.Information(abiStructsWithBaseField.Name);
            //}

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

            //if (abiStruct == null)
            //    // TODO search in eosio base types, throw invalid if not found
            //    return null;

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

        ///// <summary>
        ///// Defines the CLR-Type in our ModuleBuilder as part of the Assembly
        ///// Adds/Defines Fields of CLR-Types to the defined Type
        ///// </summary>
        ///// <param name="abiStructName"></param>
        ///// <returns>the newly created Type</returns>
        //private Type CreateAbiStructClrType(string abiStructName)
        //{
        //    var abiStruct = _abiStructsWithBaseFields.FirstOrDefault(a => a.Name == abiStructName);
        //    if (abiStruct == null)
        //        // TODO search in eosio base types, throw invalid if not found
        //        return null;

        //    // Guess we should check that a type-name/class name is not longer than 511 characters + some other checks
        //    // https://stackoverflow.com/questions/186523/what-is-the-maximum-length-of-a-c-cli-identifier
        //    var clrTypeName = $"{_contractName}_{abiStruct.Name}";
        //    var dynamicType = _moduleBuilder.DefineType(clrTypeName,
        //        TypeAttributes.Public |
        //        TypeAttributes.Sealed |
        //        TypeAttributes.SequentialLayout |
        //        TypeAttributes.Serializable,
        //        typeof(ValueType));

        //    foreach (var abiField in abiStruct.Fields)
        //    {
        //        var (abiFieldType, isOptional) = GetClrAbiFieldTypeInfo(abiField);

        //        var fieldBuilder = dynamicType.DefineField(abiField.Name, abiFieldType, FieldAttributes.Public);
        //        if (isOptional)
        //            fieldBuilder.SetCustomAttribute(NullableHelper.NullableAttributeBuilder);
        //    }

        //    return dynamicType.CreateType()!;
        //}

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

            // TODO EOSIO base types ?
            var clrTypeName = $"{fieldTypeName}";

            if (TypeMap.TryGetValue(fieldTypeName, out var abiClrFieldType))
                isModuleType = false; // if this type was found in the TypeMap it's not a moduleType, in all other cases it is
            else
                abiClrFieldType = _moduleBuilder.GetTypes().FirstOrDefault(t => t.FullName == clrTypeName);

            if (abiClrFieldType == null)
                abiClrFieldType = CreateAbiStructClrType(fieldTypeName);

            if(abiClrFieldType == null)
                Log.Information($"TYPE {fieldTypeName} NOT FOUND");
            // TODO if still null, throw

            return (abiClrFieldType, isOptional, isArrayType, isModuleType);
        }

        static Type BinaryReaderType = typeof(BinaryReader);

        private Type? CreateAbiStructClrType(string abiStructName)
        {
            try
            {
                var abiStruct = _abiStructsWithBaseFields.FirstOrDefault(a => a.Name == abiStructName);
                if (abiStruct == null)
                {
                    Log.Information($"abiStruct {abiStructName} not found (255)");
                    // TODO search in eosio base types, throw invalid if not found
                    return null;
                }

                // Guess we should check that a type-name/class name is not longer than 511 characters + some other checks
                // https://stackoverflow.com/questions/186523/what-is-the-maximum-length-of-a-c-cli-identifier
                var clrTypeName = $"{abiStructName}";
                var dynamicType = _moduleBuilder.DefineType(clrTypeName,
                    TypeAttributes.Public |
                    TypeAttributes.Sealed |
                    TypeAttributes.SequentialLayout,
                    typeof(object));

                // Constructor
                var binaryReaderAttr = new Type[] { BinaryReaderType };
                var defaultConstructor = dynamicType.DefineDefaultConstructor(MethodAttributes.Public);
                //var defaultConstructorIlGenerator = defaultConstructor.GetILGenerator();
                var binaryReaderConstructor = dynamicType.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis | CallingConventions.Standard, binaryReaderAttr);
                var binaryReaderConstructorIlGenerator = binaryReaderConstructor.GetILGenerator();

                // Load "this"
                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);

                // Get the constructor of our new type with BinaryReader-Param so we can add generated IL-Code
                binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, defaultConstructor);

                // Iterate all Fields, writes IL-Code to set their values
                foreach (var abiField in abiStruct.Fields)
                {
                    var (abiFieldType, isOptional, isArrayType, isModuleType) = GetClrAbiFieldTypeInfo(abiField);

                    if(abiFieldType == null)
                        continue;

                    // add new field to type
//                    DefineProperty(abiField.Name, PropertyAttributes.None, CallingConventions.Standard, abiFieldType, new Type[] { });

                    if (isArrayType)
                    {
                        // make the type an array
                        var abiArrayFieldType = abiFieldType.MakeArrayType();

                        var fieldBuilder = dynamicType.DefineField(abiField.Name, abiArrayFieldType, FieldAttributes.Public);

                        var iLocal = binaryReaderConstructorIlGenerator.DeclareLocal(typeof(int)); // i
                        var condLocal = binaryReaderConstructorIlGenerator.DeclareLocal(typeof(bool)); // loop condition

                        var loopStartLabel = binaryReaderConstructorIlGenerator.DefineLabel();
                        var loopBodyLabel = binaryReaderConstructorIlGenerator.DefineLabel();

                        // get the readerMethod for this type
                        var readerMethod = GetReaderMethodForType(abiFieldType, abiField);

                        // get the constructor that takes in 1 integer (the size of the array)
                        //var arrayConstructor = abiArrayFieldType.GetConstructor(new Type[] {typeof(int)});

                        // Load "this" onto stack
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);

                        // Load BinaryReader reference onto stack
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1);

                        // Read the length of the array and put it onto the stack
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Callvirt, Read7BitEncodedInt);

                        // Call the constructor for the array, length from stack is used to initialize size
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Newarr, abiFieldType);

                        // set array field to created array
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);

                        // put value 0 as int32 onto the stack
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_0);
                        
                        // pop 0 from stack into local list at index 0
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_0);

                        // start loop
                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Br_S, loopBodyLabel);

                        binaryReaderConstructorIlGenerator.MarkLabel(loopStartLabel); // loop jumps back here

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0); // this

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);// TODO abiFieldType or abiFieldTypeArry ?!

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_0, iLocal); // i

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1); // reader

                        // Call the Reader/Deserialization-Method
                        // if this type is part of the generated module/assembly, call it's the constructor
                        // if not, call it's deserialization-method
                        if (isModuleType)
                            binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, abiFieldType.GetConstructor(new Type[] { typeof(BinaryReader) })!);
                        else
                        {
                            var meth = GetReaderMethodForType(abiFieldType, abiField);
                            if (meth.IsVirtual)
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Callvirt, meth);
                            else
                                binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, meth);
                        }

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Stelem_Ref);

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_0, iLocal); // load i

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_1); // push 1 onto stack

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Add); // add 1 to i 

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_0, iLocal); // pop i


                        binaryReaderConstructorIlGenerator.MarkLabel(loopBodyLabel); // loop initially jumps here, loop condition follows

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_0, iLocal); // load i

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0); // this

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);// TODO abiFieldType or abiFieldTypeArry ?!

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldlen); // push array length onto stack

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Conv_I4); // convert to int32

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Clt); // compare i and length

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_1, condLocal);

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_1, condLocal);

                        binaryReaderConstructorIlGenerator.Emit(OpCodes.Brtrue_S, loopStartLabel);

                        ////get the Set method that takes in 1 integer (index) and one instance of type 
                        //var set_method = abiArrayFieldType.GetMethod("Set", new[] { typeof(int), abiFieldType });

                        ////binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc, local);
                        ////binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc, local);

                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_1); // index
                        ////binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_1); // value

                        //// Load BinaryReader reference onto stack
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_1);

                        //// Call the Reader/Deserialization-Method
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, readerMethod); // value on stack

                        //// call the Set method to set the value
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, set_method);

                        //  binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc, local);

                        // Load "this" onto stack
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldarg_0);


                        //Label labelCheckCondition = binaryReaderConstructorIlGenerator.DefineLabel();
                        //Label loopNext = binaryReaderConstructorIlGenerator.DefineLabel();

                        //// Load the address of the local variable represented by
                        //// locAi, which will hold the ArgIterator.
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloca_S, locAi);


                        //// Enter the loop at the point where the remaining argument
                        //// count is tested.
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Br_S, labelCheckCondition);

                        //// At the top of the loop, call GetNextArg to get the next
                        //// argument from the ArgIterator. Convert the typed reference
                        //// to an object reference and write the object to the console.
                        //binaryReaderConstructorIlGenerator.MarkLabel(loopNext);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloca_S, locAi);


                        //binaryReaderConstructorIlGenerator.MarkLabel(labelCheckCondition);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloca_S, locAi);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, typeof(ArgIterator).GetMethod("GetRemainingCount"));

                        //// If the remaining count is greater than zero, go to
                        //// the top of the loop.
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldc_I4_0);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Cgt);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Stloc_1);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Ldloc_1);
                        //binaryReaderConstructorIlGenerator.Emit(OpCodes.Brtrue_S, loopNext);
                    }
                    else
                    {
                        var fieldBuilder = dynamicType.DefineField(abiField.Name, abiFieldType, FieldAttributes.Public);

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
                            binaryReaderConstructorIlGenerator.Emit(OpCodes.Call, abiFieldType.GetConstructor(new Type[] { typeof(BinaryReader) })!);
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
                // End of method - return
                binaryReaderConstructorIlGenerator.Emit(OpCodes.Ret);

                return dynamicType.CreateType()!;
            }
            catch (Exception e)
            {
                Log.Error(e,"");
                Log.Information($"{abiStructName}");
                //Log.Information($"TYPES: ");
                //foreach (var type in _moduleBuilder.GetTypes())
                //{
                //    Log.Information(type.Name);
                //}
            }

            return null;
        }

        public MethodInfo GetReaderMethodForType(Type type, AbiField abiField)
        {
            if (PrimitiveTypeReaderMethodMap.TryGetValue(type, out var methodInfo) || PredefinedTypesReaderMethodMap.TryGetValue(type, out methodInfo))
            {
                if (methodInfo == null)
                    Log.Information("Test");

                return methodInfo;
            }
            else
            {
                Log.Information($"ReaderMethod for {type.FullName} {abiField.Name} not found");
                return ReadByte;
            }
        }

        #region BinaryReader standard methods

        public static MethodInfo Read7BitEncodedInt = typeof(BinaryReader).GetMethod(nameof(BinaryReader.Read7BitEncodedInt))!;

        public static MethodInfo Read7BitEncodedInt64 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.Read7BitEncodedInt64))!;

        public static MethodInfo ReadBoolean = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBoolean))!;

        public static MethodInfo ReadByte = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadByte))!;

//        public static MethodInfo ReadBytes = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes))!;// needs arg

        public static MethodInfo ReadChar = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadChar))!;

        public static MethodInfo ReadChars = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadChars))!;// needs arg

        public static MethodInfo ReadDecimal = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadDecimal))!;

        public static MethodInfo ReadFloat = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadSingle))!;

        public static MethodInfo ReadDouble = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadDouble))!;

        public static MethodInfo ReadHalf = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadHalf))!;

        public static MethodInfo ReadInt16 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadInt16))!;

        public static MethodInfo ReadInt32 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadInt32))!;

        public static MethodInfo ReadInt64 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadInt64))!;

        public static MethodInfo ReadSByte = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadSByte))!;

        //public static MethodInfo ReadSingle = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadSingle))!;

        public static MethodInfo ReadString = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadString))!;

        public static MethodInfo ReadUInt16 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt16))!;

        public static MethodInfo ReadUInt32 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt32))!;

        public static MethodInfo ReadUInt64 = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt64))!;

        public static MethodInfo ReadVarBytes = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadBytes))!; // Bytes, ActionDataBytes


        public static Dictionary<string, Type> TypeMap = new Dictionary<string, Type>()
        {
            { "bool", typeof(bool) },
            { "uint8", typeof(byte) },
            { "int8", typeof(sbyte) },
            { "byte", typeof(byte) },
            { "bytes", typeof(byte[]) },
            { "char", typeof(char) },
//            { "", typeof(char[]) },
            { "decimal", typeof(decimal) },
            { "float", typeof(float) },
            { "double", typeof(double) },
            { "float32", typeof(float) },
            { "float64", typeof(double) },
//            { "", typeof(Half) },
            { "int16", typeof(Int16) },
            { "int32", typeof(Int32) },
            { "int64", typeof(Int64) },
//            { "", typeof(sbyte) },
//            { "", typeof(Single) },
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
        };

        // Primitive types (and string)
        public static Dictionary<Type, MethodInfo> PrimitiveTypeReaderMethodMap = new Dictionary<Type, MethodInfo>()
        {
            { typeof(bool), ReadBoolean },
            { typeof(byte), ReadByte },
            { typeof(sbyte), ReadSByte },
            { typeof(byte[]), ReadVarBytes },
            { typeof(char), ReadChar },
//            { typeof(char[]), ReadChars },
            { typeof(decimal), ReadDecimal },
//            { typeof(Half), ReadHalf },
            { typeof(Int16), ReadInt16 },
            { typeof(Int32), ReadInt32 },
            { typeof(Int64), ReadInt64 },
//            { typeof(Single), ReadSingle },
            { typeof(string), ReadString },
            { typeof(UInt16), ReadInt16 },
            { typeof(UInt32), ReadInt32 },
            { typeof(UInt64), ReadInt64 },
            { typeof(float), ReadFloat },
            { typeof(double), ReadDouble },
        };

        #endregion

        #region IEosioSerializable Methods

        public static MethodInfo ReadAbi = typeof(Abi).GetMethod(nameof(Abi.ReadFromBinaryReader))!;

        public static MethodInfo ReadAbiType = typeof(AbiType).GetMethod(nameof(AbiType.ReadFromBinaryReader))!;

        public static MethodInfo ReadAbiStruct = typeof(AbiStruct).GetMethod(nameof(AbiStruct.ReadFromBinaryReader))!;

        public static MethodInfo ReadAbiField = typeof(AbiField).GetMethod(nameof(AbiField.ReadFromBinaryReader))!;

        public static MethodInfo ReadAbiAction = typeof(AbiAction).GetMethod(nameof(AbiAction.ReadFromBinaryReader))!;

        public static MethodInfo ReadAbiTable = typeof(AbiTable).GetMethod(nameof(AbiTable.ReadFromBinaryReader))!;

        public static MethodInfo ReadAccountRamDelta = typeof(AccountRamDelta).GetMethod(nameof(AccountRamDelta.ReadFromBinaryReader))!;

        public static MethodInfo ReadAccountDelta = typeof(AccountDelta).GetMethod(nameof(AccountDelta.ReadFromBinaryReader))!;

        public static MethodInfo ReadAction = typeof(Action).GetMethod(nameof(Action.ReadFromBinaryReader))!;

        public static MethodInfo ReadActionReceipt = typeof(ActionReceipt).GetMethod(nameof(ActionReceipt.ReadFromBinaryReader))!;

        public static MethodInfo ReadActionTrace = typeof(ActionTrace).GetMethod(nameof(ActionTrace.ReadFromBinaryReader))!;

        public static MethodInfo ReadBlockHeader = typeof(BlockHeader).GetMethod(nameof(BlockHeader.ReadFromBinaryReader))!;

        public static MethodInfo ReadBlockSigningAuthorityVariant = typeof(BlockSigningAuthorityVariant).GetMethod(nameof(BlockSigningAuthorityVariant.ReadFromBinaryReader))!;

        public static MethodInfo ReadBlockSigningAuthorityV0 = typeof(BlockSigningAuthorityV0).GetMethod(nameof(BlockSigningAuthorityV0.ReadFromBinaryReader))!;

        public static MethodInfo ReadBlockState = typeof(BlockState).GetMethod(nameof(BlockState.ReadFromBinaryReader))!;

        public static MethodInfo ReadPairAccountNameBlockNum = typeof(PairAccountNameBlockNum).GetMethod(nameof(PairAccountNameBlockNum.ReadFromBinaryReader))!;

        public static MethodInfo ReadScheduleInfo = typeof(ScheduleInfo).GetMethod(nameof(ScheduleInfo.ReadFromBinaryReader))!;

        public static MethodInfo ReadIncrementalMerkle = typeof(IncrementalMerkle).GetMethod(nameof(IncrementalMerkle.ReadFromBinaryReader))!;

        public static MethodInfo ReadBlockHeaderState = typeof(BlockHeaderState).GetMethod(nameof(BlockHeaderState.ReadFromBinaryReader))!;

        public static MethodInfo ReadProducerAuthority = typeof(ProducerAuthority).GetMethod(nameof(ProducerAuthority.ReadFromBinaryReader))!;

        public static MethodInfo ReadProducerAuthoritySchedule = typeof(ProducerAuthoritySchedule).GetMethod(nameof(ProducerAuthoritySchedule.ReadFromBinaryReader))!;

        public static MethodInfo ReadProducerKey = typeof(ProducerKey).GetMethod(nameof(ProducerKey.ReadFromBinaryReader))!;

        public static MethodInfo ReadPackedTransaction = typeof(PackedTransaction).GetMethod(nameof(PackedTransaction.ReadFromBinaryReader))!;

        public static MethodInfo ReadPermissionLevel = typeof(PermissionLevel).GetMethod(nameof(PermissionLevel.ReadFromBinaryReader))!;

        public static MethodInfo ReadProducerSchedule = typeof(ProducerSchedule).GetMethod(nameof(ProducerSchedule.ReadFromBinaryReader))!;

        public static MethodInfo ReadProtocolFeatureActivationSet = typeof(ProtocolFeatureActivationSet).GetMethod(nameof(ProtocolFeatureActivationSet.ReadFromBinaryReader))!;

        public static MethodInfo ReadSharedKeyWeight = typeof(SharedKeyWeight).GetMethod(nameof(SharedKeyWeight.ReadFromBinaryReader))!;

        public static MethodInfo ReadSignedBlock = typeof(SignedBlock).GetMethod(nameof(SignedBlock.ReadFromBinaryReader))!;

        public static MethodInfo ReadSignedBlockHeader = typeof(SignedBlockHeader).GetMethod(nameof(SignedBlockHeader.ReadFromBinaryReader))!;

        public static MethodInfo ReadSignedTransaction = typeof(SignedTransaction).GetMethod(nameof(SignedTransaction.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransaction = typeof(Transaction).GetMethod(nameof(Transaction.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransactionHeader = typeof(TransactionHeader).GetMethod(nameof(TransactionHeader.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransactionReceipt = typeof(TransactionReceipt).GetMethod(nameof(TransactionReceipt.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransactionReceiptHeader = typeof(TransactionReceiptHeader).GetMethod(nameof(TransactionReceiptHeader.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransactionTrace = typeof(TransactionTrace).GetMethod(nameof(TransactionTrace.ReadFromBinaryReader))!;

        public static MethodInfo ReadExcept = typeof(Except).GetMethod(nameof(Except.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransactionTraceAuthSequence = typeof(TransactionTraceAuthSequence).GetMethod(nameof(TransactionTraceAuthSequence.ReadFromBinaryReader))!;

        public static MethodInfo ReadTransactionVariant = typeof(TransactionVariant).GetMethod(nameof(TransactionVariant.ReadFromBinaryReader))!;

        // TODO better names for these regions
        #region EosTypes BinaryReaderExtension Methods 

        public static MethodInfo ReadAsset = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadAsset))!;

        public static MethodInfo ReadChecksum160 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadChecksum160))!;

        public static MethodInfo ReadChecksum256 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadChecksum256))!;

        public static MethodInfo ReadChecksum512 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadChecksum512))!;

        // ExtendedAsset

        public static MethodInfo ReadFloat128 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadFloat128))!;

        public static MethodInfo ReadInt128 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadInt128))!;

        public static MethodInfo ReadName = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadName))!;

        public static MethodInfo ReadPublicKey = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadPublicKey))!;

        public static MethodInfo ReadSymbol = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadSymbol))!;

        public static MethodInfo ReadSymbolCode = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadSymbolCode))!;

        public static MethodInfo ReadTimestamp = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadTimestamp))!;

        public static MethodInfo ReadUInt128 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadUInt128))!;

        public static MethodInfo ReadAuthority = typeof(Authority).GetMethod(nameof(Authority.ReadFromBinaryReader))!;


        public static MethodInfo ReadVarUint32 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadVarUint32))!;

        public static MethodInfo ReadVarUint64 = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadVarUint64))!;


        public static MethodInfo ReadKeyWeight = typeof(KeyWeight).GetMethod(nameof(KeyWeight.ReadFromBinaryReader))!;

        public static MethodInfo ReadWaitWeight = typeof(WaitWeight).GetMethod(nameof(WaitWeight.ReadFromBinaryReader))!;
        
        public static MethodInfo ReadPermissionLevelWeight = typeof(PermissionLevelWeight).GetMethod(nameof(PermissionLevelWeight.ReadFromBinaryReader))!;

        public static MethodInfo ReadExtension = typeof(BinaryReaderExtensions).GetMethod(nameof(BinaryReaderExtensions.ReadExtension))!;

        #endregion

        // all EosioSerializable-Types
        public static Dictionary<Type, MethodInfo> PredefinedTypesReaderMethodMap = new Dictionary<Type, MethodInfo>()
        {
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

            public static CustomAttributeBuilder GetNullableAttributeBuilder()
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
