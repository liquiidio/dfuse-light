using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class AccountDelta : IEosioSerializable<AccountDelta>
{
    public Name Account = string.Empty;
    public long Delta = 0;

    public static AccountDelta ReadFromBinaryReader(BinaryReader reader)
    {
        var accountDelta = new AccountDelta()
        {
            Account = reader.ReadName(),
            Delta = reader.ReadInt64(),
        };

        return accountDelta;
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}