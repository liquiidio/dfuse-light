using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public sealed class AccountRamDelta : IEosioSerializable<AccountRamDelta>
{
    public Name Account;// string
    public long Delta;// int64

    public AccountRamDelta(BinaryReader reader)
    {
        Account = Name.ReadFromBinaryReader(reader);
        Delta = reader.ReadInt64();
    }

    public static AccountRamDelta ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        return new AccountRamDelta(reader);
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        Account.WriteToBinaryWriter(writer); // TODO Eosio Name
        writer.Write(Delta); // TODO VARINT
    }
}