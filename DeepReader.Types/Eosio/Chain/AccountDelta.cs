using DeepReader.Types.EosTypes;

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
            Account = Name.ReadFromBinaryReader(reader); // TODO
            Delta = reader.ReadInt64();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static AccountDelta ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new AccountDelta(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}