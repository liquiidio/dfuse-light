using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types;

public class AccountRamDelta : IEosioSerializable<AccountRamDelta>
{
    public Name Account = string.Empty;// string
    public long Delta = 0;// int64

    public static AccountRamDelta ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo @corvin from haron
        // Haron: "This class is in DeepReader.Types namespace, also have a look at string Account"
        var accountRamDelta = new AccountRamDelta()
        {
            Account = reader.ReadName(),
            Delta = reader.ReadInt64()
        };
        return accountRamDelta;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.WriteName(Account); // TODO Eosio Name
        writer.Write(Delta); // TODO VARINT
    }
}