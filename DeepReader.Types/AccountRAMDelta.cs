using DeepReader.Types.EosTypes;

namespace DeepReader.Types;

public sealed class AccountRamDelta : IEosioSerializable<AccountRamDelta>, IFasterSerializable<AccountRamDelta>
{
    public Name Account;// string
    public long Delta;// int64

    public AccountRamDelta() { }

    public AccountRamDelta(IBufferReader reader)
    {
        Account = Name.ReadFromBinaryReader(reader);
        Delta = reader.ReadInt64();
    }

    public static AccountRamDelta ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new AccountRamDelta(reader);
    }

    public static AccountRamDelta ReadFromFaster(BinaryReader reader, bool fromPool = true)
    {
        var obj = new AccountRamDelta();

        obj.Account = Name.ReadFromFaster(reader);
        obj.Delta = reader.ReadInt64();

        return obj;
    }

    public void WriteToFaster(BinaryWriter writer)
    {
        Account.WriteToFaster(writer); // TODO Eosio Name
        writer.Write(Delta); // TODO VARINT
    }
}