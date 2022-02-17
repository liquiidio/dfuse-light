using System.Text;
using Cryptography.ECDSA;

namespace DeepReader.Types.Helpers;

public static class CryptoHelper
{
    /// <summary>
    /// Convert byte array to encoded public key string
    /// </summary>
    /// <param name="keyBytes">public key bytes</param>
    /// <param name="keyType">Optional key type. (sha256x2, R1, K1)</param>
    /// <param name="prefix">Optional prefix to public key</param>
    /// <returns>encoded public key</returns>
    public static string PubKeyBytesToString(byte[] keyBytes, string? keyType = null, string prefix = "EOS")
    {
        return KeyToString(keyBytes, keyType, prefix);
    }

    /// <summary>
    /// Convert key byte array to encoded generic key
    /// </summary>
    /// <param name="key">key byte array</param>
    /// <param name="keyType">Key type. (sha256x2, R1, K1)</param>
    /// <param name="prefix">Optional prefix</param>
    /// <returns></returns>
    public static string KeyToString(byte[] key, string? keyType, string? prefix = null)
    {
        byte[] digest;

        if (keyType == "sha256x2")
        {
            digest = Sha256Manager.GetHash(Sha256Manager.GetHash(SerializationHelper.Combine(new List<byte[]>() {
                new byte[] { 128 },
                key
            })));
        }
        else if (!string.IsNullOrWhiteSpace(keyType))
        {
            digest = Ripemd160Manager.GetHash(SerializationHelper.Combine(new List<byte[]>() {
                key,
                Encoding.UTF8.GetBytes(keyType)
            }));
        }
        else
        {
            digest = Ripemd160Manager.GetHash(key);
        }

        if (keyType == "sha256x2")
        {
            return (prefix ?? "") + Base58.Encode(SerializationHelper.Combine(new List<byte[]>() {
                new byte[] { 128 },
                key,
                digest.Take(4).ToArray()
            }));
        }
        else
        {
            return (prefix ?? "") + Base58.Encode(SerializationHelper.Combine(new List<byte[]>() {
                key,
                digest.Take(4).ToArray()
            }));
        }
    }

    /// <summary>
    /// Convert byte array to encoded signature string
    /// </summary>
    /// <param name="signBytes">signature bytes</param>
    /// <param name="keyType">Optional key type. (sha256x2, R1, K1)</param>
    /// <param name="prefix">Optional prefix to public key</param>
    /// <returns>encoded signature</returns>
    public static string SignBytesToString(byte[] signBytes, string keyType = "K1", string prefix = "SIG_K1_")
    {
        return KeyToString(signBytes, keyType, prefix);
    }

}