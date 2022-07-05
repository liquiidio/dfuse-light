using DeepReader.Types.EosTypes;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Infrastructure.BinaryWriters;
using DeepReader.Types.Interfaces;

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
            Account = Name.ReadFromBinaryReader(reader);
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

    public static AccountDelta ReadFromFaster(IBufferReader reader, bool fromPool = true)
    {
        var obj = new AccountDelta();

        obj.Account = Name.ReadFromFaster(reader);
        obj.Delta = reader.ReadInt64();

        return obj;
    }

    public void WriteToFaster(IBufferWriter writer)
    {
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}