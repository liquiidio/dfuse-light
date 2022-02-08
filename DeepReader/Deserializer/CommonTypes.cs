using DeepReader.EosTypes;
using DeepReader.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.AssemblyGenerator;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Deserializer
{
    public static class CommonTypes
    {

        public static readonly Type TypeOfString = typeof(string);
        public static readonly Type TypeOfBoolean = typeof(bool);
        public static readonly Type TypeOfByte = typeof(byte);
        public static readonly Type TypeOfSbyte = typeof(sbyte);
        public static readonly Type TypeOfChar = typeof(char);
        public static readonly Type TypeOfDecimal = typeof(decimal);
        public static readonly Type TypeOfFloat = typeof(float);
        public static readonly Type TypeOfDouble = typeof(double);
        public static readonly Type TypeOfFloat128 = typeof(Float128);

        public static readonly Type TypeOfInt = typeof(int);
        public static readonly Type TypeOfUint = typeof(uint);
        public static readonly Type TypeOfLong = typeof(long);
        public static readonly Type TypeOfUlong = typeof(ulong);
        public static readonly Type TypeOfShort = typeof(short);
        public static readonly Type TypeOfUshort = typeof(ushort);

        public static readonly Type TypeOfVarUint32 = typeof(VarUint32);
        public static readonly Type TypeOfVarInt32 = typeof(VarInt32);
        public static readonly Type TypeOfVarUint64 = typeof(VarUint64);
        public static readonly Type TypeOfVarInt64 = typeof(VarInt64);
        public static readonly Type TypeOfChecksum160 = typeof(Checksum160);
        public static readonly Type TypeOfChecksum256 = typeof(Checksum256);
        public static readonly Type TypeOfChecksum512 = typeof(Checksum512);
        public static readonly Type TypeOfSignature = typeof(Signature);
        public static readonly Type TypeOfUint128 = typeof(Uint128);
        public static readonly Type TypeOfInt128 = typeof(Int128);
        public static readonly Type TypeOfName = typeof(Name);
        public static readonly Type TypeOfBytes = typeof(Bytes);
        public static readonly Type TypeOfPublicKey = typeof(PublicKey);

        public static readonly Type TypeOfAsset = typeof(Asset);
        public static readonly Type TypeOfExtendedAsset = typeof(ExtendedAsset);
        public static readonly Type TypeOfSymbol = typeof(Symbol);
        public static readonly Type TypeOfSymbolCode = typeof(SymbolCode);

        public static readonly Type TypeOfActionDataBytes = typeof(ActionDataBytes);
        //public static readonly Type TypeOfTracesBytes = typeof(TracesBytes);
        //public static readonly Type TypeOfDeltasBytes = typeof(DeltasBytes);
        //public static readonly Type TypeOfBlockBytes = typeof(BlockBytes);
        //public static readonly Type TypeOfContractRowBytes = typeof(ContractRowBytes);
        //public static readonly Type TypeOfActionBytes = typeof(ActionBytes);
        //public static readonly Type TypeOfPackedTransactionBytes = typeof(PackedTransactionBytes);

        //public static readonly Type TypeOfTransactionVariant = typeof(Variants.TransactionVariant);
        //public static readonly Type TypeOfTransactionTraceVariant = typeof(TransactionTrace);
        //public static readonly Type TypeOfTableDeltaVariant = typeof(TableDelta);
        //public static readonly Type TypeOfActionTraceVariant = typeof(ActionTrace);
        //public static readonly Type TypeOfPartialTransactionVariant = typeof(PartialTransaction);
        //public static readonly Type TypeOfActionReceiptVariant = typeof(ActionReceipt);
        //public static readonly Type TypeOfRequestVariant = typeof(Request);
        //public static readonly Type TypeOfResultVariant = typeof(Result);
        //public static readonly Type TypeOfAccountVariant = typeof(Account);
        //public static readonly Type TypeOfAccountMetadataVariant = typeof(AccountMetadata);
        //public static readonly Type TypeOfCodeVariant = typeof(Code);
        //public static readonly Type TypeOfContractTableVariant = typeof(ContractTable);
        //public static readonly Type TypeOfContractRowVariant = typeof(ContractRow);
        //public static readonly Type TypeOfContractIndex64Variant = typeof(ContractIndex64);
        //public static readonly Type TypeOfContractIndex128Variant = typeof(ContractIndex128);
        //public static readonly Type TypeOfContractIndex256Variant = typeof(ContractIndex256);
        //public static readonly Type TypeOfContractIndexDoubleVariant = typeof(ContractIndexDouble);
        //public static readonly Type TypeOfContractIndexLongDoubleVariant = typeof(ContractIndexLongDouble);
        //public static readonly Type TypeOfChainConfigVariant = typeof(ChainConfig);
        //public static readonly Type TypeOfGlobalPropertyVariant = typeof(GlobalProperty);
        //public static readonly Type TypeOfGeneratedTransactionVariant = typeof(GeneratedTransaction);
        //public static readonly Type TypeOfActivatedProtocolFeatureVariant = typeof(ActivatedProtocolFeature);
        //public static readonly Type TypeOfProtocolStateVariant = typeof(ProtocolState);
        //public static readonly Type TypeOfPermissionVariant = typeof(Permission);
        //public static readonly Type TypeOfPermissionLinkVariant = typeof(PermissionLink);
        //public static readonly Type TypeOfResourceLimitsVariant = typeof(ResourceLimits);
        //public static readonly Type TypeOfUsageAccumulatorVariant = typeof(UsageAccumulator);
        //public static readonly Type TypeOfResourceUsageVariant = typeof(ResourceUsage);
        //public static readonly Type TypeOfResourceLimitsStateVariant = typeof(ResourceLimitsState);
        //public static readonly Type TypeOfResourceLimitsRatioVariant = typeof(ResourceLimitsRatio);
        //public static readonly Type TypeOfElasticLimitParametersVariant = typeof(ElasticLimitParameters);
        //public static readonly Type TypeOfResourceLimitsConfigVariant = typeof(ResourceLimitsConfig);
        //public static readonly Type TypeOfBlockSigningAuthorityVariant = typeof(BlockSigningAuthority);

        //internal static readonly Type TypeOfTransactionTraceV0 = typeof(TransactionTraceV0);
        //internal static readonly Type TypeOfTableDeltaV0 = typeof(TableDeltaV0);
        //internal static readonly Type TypeOfActionTraceV0 = typeof(ActionTraceV0);
        //internal static readonly Type TypeOfPartialTransactionV0 = typeof(PartialTransactionV0);
        //internal static readonly Type TypeOfActionReceiptV0 = typeof(ActionReceiptV0);
        //internal static readonly Type TypeOfGetStatusRequestV0 = typeof(GetStatusRequestV0);
        //internal static readonly Type TypeOfGetBlocksRequestV0 = typeof(GetBlocksRequestV0);
        //internal static readonly Type TypeOfGetBlocksAckRequestV0 = typeof(GetBlocksAckRequestV0);
        //internal static readonly Type TypeOfGetStatusResultV0 = typeof(GetStatusResultV0);
        //internal static readonly Type TypeOfGetBlocksResultV0 = typeof(GetBlocksResultV0);
        //internal static readonly Type TypeOfAccountV0 = typeof(AccountV0);
        //internal static readonly Type TypeOfAccountMetadataV0 = typeof(AccountMetadataV0);
        //internal static readonly Type TypeOfCodeV0 = typeof(CodeV0);
        //internal static readonly Type TypeOfContractTableV0 = typeof(ContractTableV0);
        //internal static readonly Type TypeOfContractRowV0 = typeof(ContractRowV0);
        //internal static readonly Type TypeOfContractIndex64V0 = typeof(ContractIndex64V0);
        //internal static readonly Type TypeOfContractIndex128V0 = typeof(ContractIndex128V0);
        //internal static readonly Type TypeOfContractIndex256V0 = typeof(ContractIndex256V0);
        //internal static readonly Type TypeOfContractIndexDoubleV0 = typeof(ContractIndexDoubleV0);
        //internal static readonly Type TypeOfContractIndexLongDoubleV0 = typeof(ContractIndexLongDoubleV0);
        //internal static readonly Type TypeOfChainConfigV0 = typeof(ChainConfigV0);
        //internal static readonly Type TypeOfGlobalPropertyV0 = typeof(GlobalPropertyV0);
        //internal static readonly Type TypeOfGlobalPropertyV1 = typeof(GlobalPropertyV1);
        //internal static readonly Type TypeOfGeneratedTransactionV0 = typeof(GeneratedTransactionV0);
        //internal static readonly Type TypeOfActivatedProtocolFeatureV0 = typeof(ActivatedProtocolFeatureV0);
        //internal static readonly Type TypeOfProtocolStateV0 = typeof(ProtocolStateV0);
        //internal static readonly Type TypeOfPermissionV0 = typeof(PermissionV0);
        //internal static readonly Type TypeOfPermissionLinkV0 = typeof(PermissionLinkV0);
        //internal static readonly Type TypeOfResourceLimitsV0 = typeof(ResourceLimitsV0);
        //internal static readonly Type TypeOfUsageAccumulatorV0 = typeof(UsageAccumulatorV0);
        //internal static readonly Type TypeOfResourceUsageV0 = typeof(ResourceUsageV0);
        //internal static readonly Type TypeOfResourceLimitsStateV0 = typeof(ResourceLimitsStateV0);
        //internal static readonly Type TypeOfResourceLimitsRatioV0 = typeof(ResourceLimitsRatioV0);
        //internal static readonly Type TypeOfElasticLimitParametersV0 = typeof(ElasticLimitParametersV0);
        //internal static readonly Type TypeOfResourceLimitsConfigV0 = typeof(ResourceLimitsConfigV0);
        //internal static readonly Type TypeOfBlockSigningAuthorityV0 = typeof(BlockSigningAuthorityV0);
        //internal static readonly Type TypeOfPackedTransaction = typeof(PackedTransactionVariant);

        internal static readonly Type TypeOfTransactionVariant = typeof(TransactionVariant);
        internal static readonly Type TypeOfPackedTransaction = typeof(PackedTransaction);
        internal static readonly Type TypeOfTransactionId = typeof(TransactionId);

        internal static readonly Type TypeOfBlockSigningAuthorityVariant = typeof(BlockSigningAuthorityVariant);
        internal static readonly Type TypeOfBlockSigningAuthorityV0 = typeof(BlockSigningAuthorityV0);


        internal static readonly Type TypeOfTimestamp = typeof(Timestamp);

        internal static readonly Type TypeOfSortOrderAttribute = typeof(SortOrderAttribute);
        internal static readonly Type TypeOfAbi = typeof(Abi);

        //        internal static readonly Type TypeOfTransaction = typeof(Transaction);
    }
}
