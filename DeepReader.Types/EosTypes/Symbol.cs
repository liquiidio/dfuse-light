namespace DeepReader.Types.EosTypes;

public struct Symbol
{
    public byte[] Binary = Array.Empty<byte>();

    public SymbolCode Code;

    public byte Precision;

    public Symbol(SymbolCode code, byte precision)
    {
        Code = code;
        Precision = precision;
    }
}