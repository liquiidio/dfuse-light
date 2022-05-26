using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public sealed class AccountDelta : IEosioSerializable<AccountDelta>
{
    public Name Account { get; set; }
    public long Delta { get; set; }

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
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}