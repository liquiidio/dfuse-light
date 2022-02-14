using System.Text;
using DeepReader.Helpers;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;
using Salar.BinaryBuffers;
using Serilog;

namespace DeepReader.DeepMindDeserializer;

public static class BinaryBufferReaderExtensions
{
    internal static class Constants
    {
        public const int PubKeyDataSize = 33;
        public const int SignKeyDataSize = 64;
    }

    public enum KeyType
    {
        K1 = 0,
        R1 = 1,
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
        return ByteArrayToHexString(reader.ReadBytes(20));
    }

    public static Checksum256 ReadChecksum256(this BinaryBufferReader reader)
    {
        return ByteArrayToHexString(reader.ReadBytes(32));
    }

    public static TransactionId ReadTransactionId(this BinaryBufferReader reader)
    {
        return ByteArrayToHexString(reader.ReadBytes(32));
    }

    public static Checksum512 ReadChecksum512(this BinaryBufferReader reader)
    {
        return ByteArrayToHexString(reader.ReadBytes(64));
    }

    public static VarUint32 ReadVarUint32(this BinaryBufferReader reader)
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

        //            return Convert.ToUInt32(reader.Read7BitEncodedInt());
    }

    public static VarInt32 ReadVarInt32(this BinaryBufferReader reader)
    {
        return reader.Read7BitEncodedInt();
    }

    public static VarUint64 ReadVarUint64(this BinaryBufferReader reader)
    {
        return (ulong)reader.Read7BitEncodedInt64();
    }

    public static VarInt64 ReadVarInt64(this BinaryBufferReader reader)
    {
        return reader.Read7BitEncodedInt64();
    }

    public static Uint128 ReadUInt128(this BinaryBufferReader reader)
    {
        return reader.ReadBytes(16);
    }

    public static Int128 ReadInt128(this BinaryBufferReader reader)
    {
        return reader.ReadBytes(8);
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

    private static readonly char[] CharMap = new[] { '.', '1', '2', '3', '4', '5', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    public static Name ReadName(this BinaryBufferReader reader)
    {
        var binary = reader.ReadUInt64();

        var str = new[] { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' };

        try
        {
            var tmp = binary;
            for (uint i = 0; i <= 12; ++i)
            {
                var c = CharMap[tmp & (ulong)(i == 0 ? 0x0f : 0x1f)];
                str[(int)(12 - i)] = c;
                tmp >>= i == 0 ? 4 : 5;
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "");
        }

        return new Name(binary, new string(str).TrimEnd('.'));
    }

    public static string ReadEosString(this BinaryBufferReader reader)
    {
        var length = Convert.ToInt32(reader.ReadVarUint32());
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


    /// <summary>
    /// Encode byte array to hexadecimal string
    /// </summary>
    /// <param name="ba">byte array to convert</param>
    /// <returns></returns>
    private static string ByteArrayToHexString(byte[] ba)
    {
        var hex = new StringBuilder(ba.Length * 2);
        foreach (var b in ba)
            hex.AppendFormat("{0:x2}", b);

        return hex.ToString();
    }

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
        return reader.ReadBytes(16);
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
        var value = new Symbol()
        {
            Precision = reader.ReadByte()
        };

        var a = reader.ReadBytes(7);

        int len;
        for (len = 0; len < a.Length; ++len)
            if (a[len] == 0)
                break;

        value.Name = string.Join("", a.Take(len).Select(b => (char)b));

        return value;
    }

    public static SymbolCode ReadSymbolCode(this BinaryBufferReader reader)
    {
        var a = reader.ReadBytes(8);

        int len;
        for (len = 0; len < a.Length; ++len)
            if (a[len] == 0)
                break;

        return new SymbolCode() { Value = string.Join("", a.Take(len)) };
    }

    public static Timestamp ReadTimestamp(this BinaryBufferReader reader)
    {
        return new Timestamp(reader.ReadUInt32());
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
}