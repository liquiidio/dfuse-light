using DeepReader.Types.EosTypes;
 
namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public sealed class AccountDelta : IEosioSerializable<AccountDelta>, IFasterSerializable<AccountDelta>
{
    public Name Account { get; set; }
    public long Delta { get; set; }

    public AccountDelta() { }

    public AccountDelta(IBufferReader reader)
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

    public static AccountDelta ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new AccountDelta(reader);
    }

    public static AccountDelta ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = new AccountDelta();

        obj.Account = Name.ReadFromFaster(reader); // TODO
        obj.Delta = reader.ReadInt64();

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}