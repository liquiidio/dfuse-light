using Salar.BinaryBuffers;

namespace DeepReader.Types.EosTypes;

public struct Symbol : IEosioSerializable<Symbol>
{
    public byte[] Binary = Array.Empty<byte>();

    public SymbolCode Code;

    public byte Precision;

    public Symbol(SymbolCode code, byte precision)
    {
        Code = code;
        Precision = precision;
    }

    public static Symbol ReadFromBinaryReader(BinaryBufferReader reader, bool fromPool = true)
    {
        var symbol = new Symbol
        {
            Precision = reader.ReadByte(),
            Code = SymbolCode.ReadFromBinaryReader(reader)
        };
        return symbol;
    }
}