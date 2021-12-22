namespace DeepReader.Types;

public class ProducerAuthority
{
    public string AccountName;//           AccountName            `json:"producer_name"`
    public BlockSigningAuthority BlockSigningAuthority;// *BlockSigningAuthority `json:"authority"`
}