using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/protocol_feature_activation.hpp
/// </summary>
public class ProtocolFeatureActivationSet : IEosioSerializable<ProtocolFeatureActivationSet>
{
    public Checksum256[] ProtocolFeatures;

    public ProtocolFeatureActivationSet(BinaryReader reader)
    {
        ProtocolFeatures = new Checksum256[reader.Read7BitEncodedInt()];

        for (int i = 0; i < ProtocolFeatures.Length; i++)
        {
            ProtocolFeatures[i] = reader.ReadChecksum256();
        }
    }
    public static ProtocolFeatureActivationSet ReadFromBinaryReader(BinaryReader reader)
    {
        return new ProtocolFeatureActivationSet(reader);
    }
}