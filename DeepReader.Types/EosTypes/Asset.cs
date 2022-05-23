using DeepReader.Types.Helpers;

namespace DeepReader.Types.EosTypes;

public struct Asset
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
}