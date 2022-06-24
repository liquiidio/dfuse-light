﻿using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Helpers;
using Serilog;
using System.Text;

namespace DeepReader.Types.Extensions
{
    public static class BinaryBufferReaderExtensions
    {
        public enum KeyType
        {
            K1 = 0,
            R1 = 1,
            WA = 2,
        };

        #region EosTypes

        public static Signature ReadSignature(this BinaryBufferReader reader)
        {
            var type = reader.ReadByte();
            var signBytes = reader.ReadBytes(Constants.SignKeyDataSize);
            reader.ReadByte();//read another byte

            switch (type)
            {
                case (int)KeyType.R1:
                    return CryptoHelper.SignBytesToString(signBytes, "R1", "SIG_R1_");
                case (int)KeyType.K1:
                    return CryptoHelper.SignBytesToString(signBytes, "K1", "SIG_K1_");
                default:
                    Log.Error(new Exception($"Signature type {type} not supported"), "");
                    Log.Error(new Exception(CryptoHelper.SignBytesToString(signBytes, "K1", "SIG_K1_")), "");
                    return CryptoHelper.SignBytesToString(signBytes, "K1", "SIG_K1_");  // TODO ??
            }
        }

        public static Checksum160 ReadChecksum160(this BinaryBufferReader reader)
        {
            return reader.ReadBytes(20);
        }

        public static Checksum256 ReadChecksum256(this BinaryBufferReader reader)
        {
            return reader.ReadBytes(32);
        }

        public static TransactionId ReadTransactionId(this BinaryBufferReader reader)
        {
            return reader.ReadBytes(32);
        }

        public static Checksum512 ReadChecksum512(this BinaryBufferReader reader)
        {
            return reader.ReadBytes(64);
        }

        public static ushort ReadVarUint16(this BinaryBufferReader reader)
        {
            ushort v = 0;
            var bit = 0;
            while (true)
            {
                var b = reader.ReadByte();
                v |= (ushort)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return (ushort)(v >> 0);
        }

        public static short ReadVarInt16(this BinaryBufferReader reader)
        {
            short v = 0;
            var bit = 0;
            while (true)
            {
                var b = reader.ReadByte();
                v |= (short)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return (short)(v >> 0);
        }

        public static uint ReadVarUint32(this BinaryBufferReader reader)
        {
            uint v = 0;
            var bit = 0;
            while (true)
            {
                var b = reader.ReadByte();
                v |= (uint)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public static int ReadVarInt32(this BinaryBufferReader reader)
        {
            int v = 0;
            var bit = 0;
            while (true)
            {
                var b = reader.ReadByte();
                v |= (int)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public static ulong ReadVarUint64(this BinaryBufferReader reader)
        {
            ulong v = 0;
            var bit = 0;
            while (true)
            {
                var b = reader.ReadByte();
                ulong v1 = (ulong)((b & 0x7f) << bit);
                v |= v1;
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public static long ReadVarInt64(this BinaryBufferReader reader)
        {
            long v = 0;
            var bit = 0;
            while (true)
            {
                var b = reader.ReadByte();
                v |= (long)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public static VarUint32 ReadVarUint32Obj(this BinaryBufferReader reader)
        {
            return reader.ReadVarUint32();
        }

        public static VarInt32 ReadVarInt32Obj(this BinaryBufferReader reader)
        {
            return reader.ReadVarInt32();
        }

        public static VarUint64 ReadVarUint64Obj(this BinaryBufferReader reader)
        {
            return reader.ReadVarUint64();
        }

        public static VarInt64 ReadVarInt64Obj(this BinaryBufferReader reader)
        {
            return reader.ReadVarInt64();
        }

        public static Uint128 ReadUInt128(this BinaryBufferReader reader)
        {
            return reader.ReadBytes(16);
        }

        public static Int128 ReadInt128(this BinaryBufferReader reader)
        {
            return reader.ReadBytes(16);
        }

        /// <summary>
        /// Slower, old ReadName-Method
        /// </summary>
        //public static Name ReadNameOld(this BinaryBufferReader reader)
        //{
        //    var a = reader.ReadBytes(8);
        //    var result = "";

        //    for (var bit = 63; bit >= 0;)
        //    {
        //        var c = 0;
        //        for (var i = 0; i < 5; ++i)
        //        {
        //            if (bit >= 0)
        //            {
        //                var idx = (int)Math.Floor((double)bit / 8);
        //                c = c << 1 | a[idx] >> bit % 8 & 1;
        //                --bit;
        //            }
        //        }
        //        if (c >= 6)
        //            result += (char)(c + 'a' - 6);
        //        else if (c >= 1)
        //            result += (char)(c + '1' - 1);
        //        else
        //            result += '.';
        //    }

        //    if (result == ".............")
        //        return new Name(Convert.ToUInt64(a), result);

        //    while (result.EndsWith("."))
        //        result = result.Substring(0, result.Length - 1);

        //    return new Name(Convert.ToUInt64(a), result);
        //}


        public static Name ReadName(this BinaryBufferReader reader)
        {
            var binary = reader.ReadBytes(8);

            //return SerializationHelper.ByteArrayToName(binary);
            return Name.ReadFromBinaryReader(reader);
        }

        public static string ReadString(this BinaryBufferReader reader)
        {
            var length = Convert.ToInt32(reader.ReadVarLength<int>());
            return length > 0 ? Encoding.UTF8.GetString(reader.ReadBytes(length)) : string.Empty;
        }

        public static Bytes ReadBytes(this BinaryBufferReader reader)
        {
            var length = Convert.ToInt32(reader.ReadVarUint32());
            return reader.ReadBytes(length);
        }

        public static PublicKey ReadPublicKey(this BinaryBufferReader reader)
        {
            var type = reader.ReadByte();
            var keyBytes = reader.ReadBytes(Constants.PubKeyDataSize);

            switch (type)
            {
                case (int)KeyType.K1:
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "K1");
                case (int)KeyType.R1:
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_");
                case (int)KeyType.WA:
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "WA", "PUB_WA_");
                default:
                    Log.Error(new Exception($"public key type {type} not supported"), "");
                    Log.Error(CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_"));
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_"); // TODO ??
            }
        }
        #endregion EosTypes


        public static ActionDataBytes ReadActionDataBytes(this BinaryBufferReader reader)
        {
            var length = Convert.ToInt32(reader.ReadVarUint32());
            return new ActionDataBytes(reader.ReadBytes(length));
        }

        //public static TracesBytes ReadTracesBytes(this BinaryBufferReader reader)
        //{
        //    var length = Convert.ToInt32(reader.ReadVarUint32());
        //    return new TracesBytes(reader.ReadBytes(length));
        //}

        //public static DeltasBytes ReadDeltasBytes(this BinaryBufferReader reader)
        //{
        //    var length = Convert.ToInt32(reader.ReadVarUint32());
        //    return new DeltasBytes(reader.ReadBytes(length));
        //}

        //public static BlockBytes ReadBlockBytes(this BinaryBufferReader reader)
        //{
        //    var length = Convert.ToInt32(reader.ReadVarUint32());
        //    return new BlockBytes(reader.ReadBytes(length));
        //}

        //public static ContractRowBytes ReadContractRowBytes(this BinaryBufferReader reader)
        //{
        //    var length = Convert.ToInt32(reader.ReadVarUint32());
        //    return new ContractRowBytes(reader.ReadBytes(length));
        //}

        //public static ActionBytes ReadActionBytes(this BinaryBufferReader reader)
        //{
        //    var length = Convert.ToInt32(reader.ReadVarUint32());
        //    return new ActionBytes(reader.ReadBytes(length));
        //}

        //public static PackedTransactionBytes ReadPackedTransactionBytes(this BinaryBufferReader reader)
        //{
        //    var length = Convert.ToInt32(reader.ReadVarUint32());
        //    return new PackedTransactionBytes(reader.ReadBytes(length));
        //}

        public static float ReadFloat32(this BinaryBufferReader reader)
        {
            return BitConverter.ToSingle(reader.ReadBytes(4));
        }

        public static double ReadFloat64(this BinaryBufferReader reader)
        {
            return BitConverter.ToDouble(reader.ReadBytes(8));
        }

        public static Float128 ReadFloat128(this BinaryBufferReader reader)
        {
            var bytes = reader.ReadBytes(16);
            return bytes;
        }

        public static Asset ReadAsset(this BinaryBufferReader reader)
        {
            var binaryAmount = reader.ReadBytes(8);

            var symbol = reader.ReadSymbol();
            var amount = SerializationHelper.SignedBinaryToDecimal(binaryAmount, symbol.Precision + 1);

            if (symbol.Precision > 0)
                amount = amount.Substring(0, amount.Length - symbol.Precision) + '.' + amount.Substring(amount.Length - symbol.Precision);

            return new Asset() { Symbol = symbol, Amount = amount, };
        }

        public static Symbol ReadSymbol(this BinaryBufferReader reader)
        {
            var precision = reader.ReadByte();

            return new Symbol(reader.ReadSymbolCode(), precision);
        }

        public static SymbolCode ReadSymbolCode(this BinaryBufferReader reader)
        {
            var a = reader.ReadBytes(8);

            int len;
            for (len = 0; len < a.Length; ++len)
                if (a[len] == 0)
                    break;

            return new SymbolCode(a, string.Join("", a.Take(len)));
        }

        public static Timestamp ReadTimestamp(this BinaryBufferReader reader)
        {
            return Timestamp.ReadFromBinaryReader(reader);
        }

        public static int Read7BitEncodedInt(this BinaryBufferReader reader)
        {
            /*
             * Copied from Microsofts BinaryReader Source-Code
             */

            // Unlike writing, we can't delegate to the 64-bit read on
            // 64-bit platforms. The reason for this is that we want to
            // stop consuming bytes if we encounter an integer overflow.

            uint result = 0;
            byte byteReadJustNow;

            // Read the integer 7 bits at a time. The high bit
            // of the byte when on means to continue reading more bytes.
            //
            // There are two failure cases: we've read more than 5 bytes,
            // or the fifth byte is about to cause integer overflow.
            // This means that we can read the first 4 bytes without
            // worrying about integer overflow.

            const int maxBytesWithoutOverflow = 4;
            for (int shift = 0; shift < maxBytesWithoutOverflow * 7; shift += 7)
            {
                // ReadByte handles end of stream cases for us.
                byteReadJustNow = reader.ReadByte();
                result |= (byteReadJustNow & 0x7Fu) << shift;

                if (byteReadJustNow <= 0x7Fu)
                {
                    return (int)result; // early exit
                }
            }

            // Read the 5th byte. Since we already read 28 bits,
            // the value of this byte must fit within 4 bits (32 - 28),
            // and it must not have the high bit set.

            byteReadJustNow = reader.ReadByte();
            if (byteReadJustNow > 0b_1111u)
            {
                throw new FormatException("Format_Bad7BitInt");
            }

            result |= (uint)byteReadJustNow << maxBytesWithoutOverflow * 7;
            return (int)result;

        }

        public static long Read7BitEncodedInt64(this BinaryBufferReader reader)
        {
            /*
             * Copied from Microsofts BinaryReader Source-Code
             */

            ulong result = 0;
            byte byteReadJustNow;

            // Read the integer 7 bits at a time. The high bit
            // of the byte when on means to continue reading more bytes.
            //
            // There are two failure cases: we've read more than 10 bytes,
            // or the tenth byte is about to cause integer overflow.
            // This means that we can read the first 9 bytes without
            // worrying about integer overflow.

            const int maxBytesWithoutOverflow = 9;
            for (int shift = 0; shift < maxBytesWithoutOverflow * 7; shift += 7)
            {
                // ReadByte handles end of stream cases for us.
                byteReadJustNow = reader.ReadByte();
                result |= (byteReadJustNow & 0x7Ful) << shift;

                if (byteReadJustNow <= 0x7Fu)
                {
                    return (long)result; // early exit
                }
            }

            // Read the 10th byte. Since we already read 63 bits,
            // the value of this byte must fit within 1 bit (64 - 63),
            // and it must not have the high bit set.

            byteReadJustNow = reader.ReadByte();
            if (byteReadJustNow > 0b_1u)
            {
                throw new FormatException("Format_Bad7BitInt");
            }

            result |= (ulong)byteReadJustNow << maxBytesWithoutOverflow * 7;
            return (long)result;

        }

        public static T ReadVarLength<T>(this BinaryBufferReader reader)
        {
            return (T)reader.ReadVarLength(typeof(T));
        }

        public static object ReadVarLength(this BinaryBufferReader reader, Type type)
        {
            if (type == CommonTypes.TypeOfShort)
                return reader.ReadVarInt16();
            else if (type == CommonTypes.TypeOfUshort)
                return reader.ReadVarUint16();
            else if (type == CommonTypes.TypeOfInt)
                return reader.ReadVarInt32();
            else if (type == CommonTypes.TypeOfUint)
                return reader.ReadVarUint32();
            else if (type == CommonTypes.TypeOfLong)
                return reader.ReadVarInt64();
            else if (type == CommonTypes.TypeOfUlong)
                return reader.ReadVarUint64();
            return null;
        }
    }

    // Todo: Move this to a differnt file later on.
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