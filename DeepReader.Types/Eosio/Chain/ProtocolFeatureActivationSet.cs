using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/protocol_feature_activation.hpp
/// </summary>
public class ProtocolFeatureActivationSet
{
    public Checksum256[] ProtocolFeatures = Array.Empty<Checksum256>();

    public static ProtocolFeatureActivationSet ReadFromBinaryReader(BinaryReader reader)
    {
        // Todo(Haron): Complete this
        return new();
    }
}