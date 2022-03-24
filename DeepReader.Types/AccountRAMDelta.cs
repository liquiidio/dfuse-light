namespace DeepReader.Types;

public class AccountRamDelta : IEosioSerializable<AccountRamDelta>
{
    public string Account = string.Empty;// string
    public long Delta = 0;// int64

    public static AccountRamDelta ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo @corvin from haron
        // Haron: "This class is in DeepReader.Types namespace, also have a look at string Account"
        var accountRamDelta = new AccountRamDelta()
        {
            Account = reader.ReadString(),
            Delta = reader.ReadInt64()
        };
        return accountRamDelta;
    }
}