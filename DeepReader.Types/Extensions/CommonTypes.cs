using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Helpers;

namespace DeepReader.Types.Extensions
{
    internal static class CommonTypes
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

        internal static readonly Type TypeOfTransactionVariant = typeof(TransactionVariant);
        internal static readonly Type TypeOfPackedTransaction = typeof(PackedTransaction);
        internal static readonly Type TypeOfTransactionId = typeof(TransactionId);

        internal static readonly Type TypeOfBlockSigningAuthorityVariant = typeof(BlockSigningAuthorityVariant);
        internal static readonly Type TypeOfBlockSigningAuthorityV0 = typeof(BlockSigningAuthorityV0);


        internal static readonly Type TypeOfTimestamp = typeof(Timestamp);

        internal static readonly Type TypeOfSortOrderAttribute = typeof(SortOrderAttribute);
        internal static readonly Type TypeOfAbi = typeof(Abi);
    }
}
