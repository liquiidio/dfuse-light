using DeepReader.EosTypes;

namespace DeepReader.Types;

//public class BlockSigningAuthority
//{
//    // Types that are valid to be assigned to Variant:
//    //	*BlockSigningAuthority_V0
//// TODO
////    public isBlockSigningAuthority_Variant Variant;//              isBlockSigningAuthority_Variant `protobuf_oneof:"variant"`
//}

public abstract class BlockSigningAuthorityVariant
{
    public abstract PublicKey PublicKey { get; }
}

public class BlockSigningAuthorityV0 : BlockSigningAuthorityVariant
{
    public uint Threshold = 0;
    public SharedKeyWeight[] Keys = Array.Empty<SharedKeyWeight>();

    public override PublicKey PublicKey => Keys?.FirstOrDefault()?.Key ?? "";
}

public class SharedKeyWeight
{
    public PublicKey Key = "";   // for now public key, is SharedPublicKey in EOSIO (see below)
    public ushort Weight;
}

//public class SharedPublicKey
//{
//    public SharedPublicKeyDataVariant PubKey;
//}

//public abstract class SharedPublicKeyDataVariant
//{

//}

////fc::ecc::public_key_shim
//public class EccPublicKeyShim : SharedPublicKeyDataVariant
//{
//    public byte[] PublicKeyData = Array.Empty<byte>();  // 33
//}

////fc::crypto::r1::public_key_shim
//public class R1PublicKeyShim : SharedPublicKeyDataVariant
//{
//    public byte[] PublicKeyData = Array.Empty<byte>();  // 33
//}

////shared_string
//public class SharedString : SharedPublicKeyDataVariant
//{
//    // TODO
//}