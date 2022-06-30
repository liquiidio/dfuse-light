using DeepReader.Types.Helpers;

namespace DeepReader.Types.EosTypes;

public struct Asset : IEosioSerializable<Asset>
{
    public long Amount;

    private string? _amountString;

    public string AmountString => _amountString ??=  AmountToString();

    public Symbol Symbol;

    private string AmountToString()
    {
        var amount = SerializationHelper.SignedBinaryToDecimal(BitConverter.GetBytes(Amount), Symbol.Precision + 1);
        if (Symbol.Precision > 0)
            amount = $"{amount[..^Symbol.Precision]}.{amount[^Symbol.Precision..]}"; // split the string at "precision", add "." between

        return amount;
    }

    public static Asset ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        var asset = new Asset();

        asset.Amount = reader.ReadInt64();

        asset.Symbol = Symbol.ReadFromBinaryReader(reader);
        //var amount = SerializationHelper.SignedBinaryToDecimal(binaryAmount, symbol.Precision + 1);

        //if (symbol.Precision > 0)
        //    amount = amount.Substring(0, amount.Length - symbol.Precision) + '.' + amount.Substring(amount.Length - symbol.Precision);

        return asset;
    }
}