namespace DeepReader.Types.EosTypes;

public struct SymbolCode
{
    public byte[] Binary = Array.Empty<byte>();

    public string Code = string.Empty;

    public SymbolCode(byte[] binary, string code)
    {
        Binary = binary;
        Code = code;
    }
}