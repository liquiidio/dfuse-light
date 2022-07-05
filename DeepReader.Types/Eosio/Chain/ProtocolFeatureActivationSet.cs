using DeepReader.Types.EosTypes;
using DeepReader.Types.Extensions;
using DeepReader.Types.Infrastructure.BinaryReaders;
using DeepReader.Types.Interfaces;

namespace DeepReader.Types.Eosio.Chain;

/// <summary>
/// libraries/chain/include/eosio/chain/protocol_feature_activation.hpp
/// </summary>
public sealed class ProtocolFeatureActivationSet : IEosioSerializable<ProtocolFeatureActivationSet>
{
    public Checksum256[] ProtocolFeatures;

    public ProtocolFeatureActivationSet(IBufferReader reader)
    {
        ProtocolFeatures = new Checksum256[reader.Read7BitEncodedInt()];

        for (int i = 0; i < ProtocolFeatures.Length; i++)
        {
            ProtocolFeatures[i] = Checksum256.ReadFromBinaryReader(reader);
        }
    }
    public static ProtocolFeatureActivationSet ReadFromBinaryReader(IBufferReader reader, bool fromPool = true)
    {
        return new ProtocolFeatureActivationSet(reader);
    }
}