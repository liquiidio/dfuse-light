using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class AccountDelta : IEosioSerializable<AccountDelta>
{
    public Name Account { get; set; }
    public long Delta { get; set; }

    public AccountDelta(BinaryReader reader)
    {
        Account = reader.ReadName();
        Delta = reader.ReadInt64();
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