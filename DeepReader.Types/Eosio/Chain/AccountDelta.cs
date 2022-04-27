using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class AccountDelta : IEosioSerializable<AccountDelta>
{
    public Name Account;
    public long Delta;

    public AccountDelta(BinaryReader reader)
    {
        try
        {
            Account = reader.ReadName(); // TODO
            Delta = reader.ReadInt64();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static AccountDelta ReadFromBinaryReader(BinaryReader reader)
    {
        return new AccountDelta(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        if (Account.Binary == null || Account.Binary.Length < 8)
        {
            string test = "";
        }
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}