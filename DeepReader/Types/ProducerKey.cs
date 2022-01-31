using DeepReader.EosTypes;

namespace DeepReader.Types;

public class ProducerKey
{
    public string AccountName = string.Empty;//AccountName
    public PublicKey[] BlockSigningKey = Array.Empty<PublicKey>();//ecc.PublicKey
}