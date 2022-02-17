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
        throw new NotImplementedException();
    }
}