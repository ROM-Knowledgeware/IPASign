
namespace IPALibrary.CodeSignature
{
    public enum HashType : byte
    {
        SHA1 = 0x01,            // CS_HASHTYPE_SHA1
        SHA256 = 0x02,          // CS_HASHTYPE_SHA256
        SHA256Truncated = 0x03, // CS_HASHTYPE_SHA256_TRUNCATED - SHA256 truncated to 20 bytes
    }
}
