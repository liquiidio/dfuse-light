using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public sealed class AccountRamDelta : IEosioSerializable<AccountRamDelta>
{
    public Name Account;// string
    public long Delta;// int64

    public AccountRamDelta(BinaryReader reader)
    {
        Account = reader.ReadName();
        Delta = reader.ReadInt64();
    }

    public static AccountRamDelta ReadFromBinaryReader(BinaryReader reader)
    {
        return new AccountRamDelta(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Account); // TODO Eosio Name
        writer.Write(Delta); // TODO VARINT
    }
}