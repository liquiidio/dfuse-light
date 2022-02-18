using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/trace.hpp
/// </summary>
public class AccountDelta {
    public Name Account = string.Empty;
    public long Delta = 0;

    public static AccountDelta ReadFromBinaryReader(BinaryReader reader)
    {
        var obj = new AccountDelta()
        {
            Account = reader.ReadUInt64(),
            Delta = reader.ReadInt64(),
        };

        return obj;
    }

    internal void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(Account.Binary);
        writer.Write(Delta);
    }
}