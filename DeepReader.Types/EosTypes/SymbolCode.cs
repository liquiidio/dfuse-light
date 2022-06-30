namespace DeepReader.Types.EosTypes;

public struct SymbolCode : IEosioSerializable<SymbolCode>
{
    public byte[] Binary = Array.Empty<byte>();

    public string Code = string.Empty;

    public SymbolCode(byte[] binary, string code)
    {
        Binary = binary;
        Code = code;
    }

    public static SymbolCode ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        var symbolCode = new SymbolCode();
        symbolCode.Binary = reader.ReadBytes(7); // this is 7 bytes as a whole symbol_code is 8bytes

        int len;
        for (len = 0; len < symbolCode.Binary.Length; ++len)
            if (symbolCode.Binary[len] == 0)
                break;
        symbolCode.Code = string.Join("", symbolCode.Binary.Take(len));
        return symbolCode;
    }
}