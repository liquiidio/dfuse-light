namespace DeepReader.Types;

public class ProducerKey
{
    public string AccountName;//     AccountName   `json:"producer_name"`
    public byte[] BlockSigningKey;// ecc.PublicKey `json:"block_signing_key"`
}