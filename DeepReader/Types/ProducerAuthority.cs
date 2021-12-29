using DeepReader.EosTypes;

namespace DeepReader.Types;

public class ProducerAuthority
{
    public Name AccountName = string.Empty;//AccountName
    public BlockSigningAuthorityVariant BlockSigningAuthority = new BlockSigningAuthorityV0();//*BlockSigningAuthority
}