using System.Text;
using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Helpers;

public class SerializationHelper
{
    /// <summary>
    /// Combina multiple arrays into one
    /// </summary>
    /// <param name="arrays"></param>
    /// <returns></returns>
    public static byte[] Combine(IEnumerable<byte[]> arrays)
    {
        byte[] ret = new byte[arrays.Sum(x => x.Length)];
        int offset = 0;
        foreach (byte[] data in arrays)
        {
            Buffer.BlockCopy(data, 0, ret, offset, data.Length);
            offset += data.Length;
        }
        return ret;
    }

    /// <summary>
    /// Encode byte array to hexadecimal string
    /// </summary>
    /// <param name="ba">byte array to convert</param>
    /// <returns></returns>
    public static string ByteArrayToHexString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);

        return hex.ToString();
    }

    public static byte[] StringToByteArray(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
        }

        return arr;
    }

    public static int GetHexVal(char hex)
    {
        int val = hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : val < 97 ? 55 : 87);
    }

    /// <summary>
    /// Convert base64 with fc prefix to byte array
    /// </summary>
    /// <param name="s">string to convert</param>
    /// <returns></returns>
    public static byte[] Base64FcStringToByteArray(string s)
    {
        //fc adds extra '='
        if ((s.Length & 3) == 1 && s[^1] == '=')
        {
            return Convert.FromBase64String(s[..^1]);
        }

        return Convert.FromBase64String(s);
    }

    /// <summary>
    /// Convert Name into unsigned long
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Converted value</returns>
    public static ulong ConvertNameToLong(string name)
    {
        return BitConverter.ToUInt64(NameToBytes(name), 0);
    }

    /// <summary>
    /// Convert Name into bytes
    /// </summary>
    /// <param name="eosName"></param>
    /// <returns>Converted value bytes</returns>
    public static byte[] NameToBytes(string eosName)
    {
        var a = new byte[8];
        var bit = 63;
        foreach (var eosChar in eosName)
        {
            var c = CharToSymbol(eosChar);
            if (bit < 5)
                c = (byte)(c << 1);
            for (var j = 4; j >= 0; --j)
            {
                if (bit >= 0)
                {
                    a[(int)Math.Floor((decimal)(bit / 8))] |= (byte)((c >> j & 1) << bit % 8);
                    --bit;
                }
            }
        }
        return a;
    }

    public static byte[] PrimaryKeyToBytes(string primaryKey)
    {
        // TODO name can be number or eosname and 64 or 128bits ?!
        return Encoding.UTF8.GetBytes(primaryKey);
    }

    /// <summary>
    /// Convert ascii char to symbol value
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static byte CharToSymbol(char c)
    {
        if (c is >= 'a' and <= 'z')
            return (byte)(c - 'a' + 6);
        if (c is >= '1' and <= '5')
            return (byte)(c - '1' + 1);
        return 0;
    }

    /// <summary>
    /// Convert big number to an signed decimal number
    /// </summary>
    /// <param name="bin">big number as byte array</param>
    /// <param name="minDigits">0-pad result to this many digits</param>
    /// <returns></returns>
    public static string SignedBinaryToDecimal(byte[] bin, int minDigits = 1)
    {
        if (IsNegative(bin))
        {
            Negate(bin);
            return '-' + BinaryToDecimal(bin, minDigits);
        }
        return BinaryToDecimal(bin, minDigits);

    }

    /// <summary>
    /// Is a big Number negative
    /// </summary>
    /// <param name="bin">big number in byte array</param>
    /// <returns></returns>
    public static bool IsNegative(byte[] bin)
    {
        return (bin[^1] & 0x80) != 0;
    }

    /// <summary>
    /// Negate a big number
    /// </summary>
    /// <param name="bin">big number in byte array</param>
    public static void Negate(byte[] bin)
    {
        var carry = 1;
        for (var i = 0; i < bin.Length; ++i)
        {
            var x = (~bin[i] & 0xff) + carry;
            bin[i] = (byte)x;
            carry = x >> 8;
        }
    }

    /// <summary>
    /// Convert an unsigned decimal number as string to a big number
    /// </summary>
    /// <param name="size">Size in bytes of the big number</param>
    /// <param name="decimalString">decimal encoded as string</param>
    /// <returns></returns>
    public static byte[] DecimalToBinary(uint size, string decimalString)
    {
        var result = new byte[size];
        foreach (var srcDigit in decimalString)
        {
            if (srcDigit is < '0' or > '9')
                throw new Exception("invalid number");
            var carry = srcDigit - '0';
            for (var j = 0; j < size; ++j)
            {
                var x = result[j] * 10 + carry;
                result[j] = (byte)x;
                carry = x >> 8;
            }
            if (carry != 0)
                throw new Exception("number is out of range");
        }
        return result;
    }

    /// <summary>
    /// Convert an signed decimal number as string to a big number
    /// </summary>
    /// <param name="size">Size in bytes of the big number</param>
    /// <param name="s">decimal encoded as string</param>
    /// <returns></returns>
    public static byte[] SignedDecimalToBinary(uint size, string s)
    {
        var negative = s[0] == '-';
        if (negative)
            s = s.Substring(0, 1);
        var result = DecimalToBinary(size, s);
        if (negative)
            Negate(result);
        return result;
    }

    /// <summary>
    /// Convert big number to an unsigned decimal number
    /// </summary>
    /// <param name="bin">big number as byte array</param>
    /// <param name="minDigits">0-pad result to this many digits</param>
    /// <returns></returns>
    public static string BinaryToDecimal(byte[] bin, int minDigits = 1)
    {
        var result = new List<char>(minDigits);

        for (var i = 0; i < minDigits; i++)
        {
            result.Add('0');
        }

        for (var i = bin.Length - 1; i >= 0; --i)
        {
            int carry = bin[i];
            for (var j = 0; j < result.Count; ++j)
            {
                var x = (result[j] - '0' << 8) + carry;
                result[j] = (char)('0' + x % 10);
                carry = x / 10 | 0;
            }
            while (carry != 0)
            {
                result.Add((char)('0' + carry % 10));
                carry = carry / 10 | 0;
            }
        }
        result.Reverse();
        return string.Join("", result);
    }

    private static readonly char[] CharMap = new[] { '.', '1', '2', '3', '4', '5', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    public static Name ByteArrayToName(byte[] binary)
    {
        var str = new[] { '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.', '.' };

        var tmp = BitConverter.ToUInt64(binary);
        for (uint i = 0; i <= 12; ++i)
        {
            var c = CharMap[tmp & (ulong)(i == 0 ? 0x0f : 0x1f)];
            str[(int)(12 - i)] = c;
            tmp >>= i == 0 ? 4 : 5;
        }

        return new Name { Binary = binary, StringVal = new string(str).TrimEnd('.') };
    }
}