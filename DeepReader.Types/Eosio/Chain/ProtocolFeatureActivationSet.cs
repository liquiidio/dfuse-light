using DeepReader.Types.EosTypes;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/protocol_feature_activation.hpp
/// </summary>
public class ProtocolFeatureActivationSet : IEosioSerializable<ProtocolFeatureActivationSet>
{
    public Checksum256[] ProtocolFeatures = Array.Empty<Checksum256>();

    public static ProtocolFeatureActivationSet ReadFromBinaryReader(BinaryReader reader)
    {
        var protocolFeatureActivationSet = new ProtocolFeatureActivationSet()
        {
            ProtocolFeatures = new Checksum256[reader.Read7BitEncodedInt()]
        };

        for (int i = 0; i < protocolFeatureActivationSet.ProtocolFeatures.Length; i++)
        {
            protocolFeatureActivationSet.ProtocolFeatures[i] = reader.ReadChecksum256();
        }
        return protocolFeatureActivationSet;
    }
}