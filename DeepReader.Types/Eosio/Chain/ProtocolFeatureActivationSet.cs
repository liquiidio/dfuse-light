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
        var obj = new ProtocolFeatureActivationSet()
        {
            ProtocolFeatures = new Checksum256[reader.ReadInt32()]
        };

        for (int i = 0; i < obj.ProtocolFeatures.Length; i++)
        {
            obj.ProtocolFeatures[i] = reader.ReadString();
        }
        return obj;
    }
}