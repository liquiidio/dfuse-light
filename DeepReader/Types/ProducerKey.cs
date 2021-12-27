namespace DeepReader.Types;

public class ProducerKey
{
    public string AccountName = string.Empty;//AccountName
    public byte[] BlockSigningKey = Array.Empty<byte>();//ecc.PublicKey
}