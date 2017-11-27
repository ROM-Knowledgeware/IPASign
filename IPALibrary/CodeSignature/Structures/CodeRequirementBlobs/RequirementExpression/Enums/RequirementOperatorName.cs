
namespace IPALibrary.CodeSignature
{
    /// <summary>
    /// Defined in requirement.h
    /// https://opensource.apple.com/source/libsecurity_codesigning/libsecurity_codesigning-55037.15/lib/requirement.h.auto.html
    /// </summary>
    public enum RequirementOperatorName : uint
    {
        False = 0,
        True = 1,
        Ident = 2,                // Match canonical code [string]
        AppleAnchor = 3,          // Signed by Apple as Apple's product
        AnchorHash = 4,           // Match anchor [cert hash]
        InfoKeyValue = 5,         // Legacy [key; value]
        And = 6,                  // Binary prefix expr AND expr [expr; expr]
        Or = 7,                   // Binary prefix expr OR expr [expr; expr]
        CodeDirectoryHash = 8,
        Not = 9,
        InfoKeyField = 10,        // Info.plist key field [string; match suffix]
        CertificateField = 11,    // Certificate field [cert index; field name; match suffix]
        TrustedCertificate = 12,  // Require trust settings to approve one particular cert [cert index]
        TrustedCertificates = 13, // Require trust settings to approve the cert chain.
        CertificateGeneric = 14,  // Certificate component by OID [cert index; oid; match suffix]
        AppleGenericAnchor = 15,  // Signed by Apple in any capacity
        EntitlementField = 16,    // Entitlement dictionary field [string; match suffix]
    }
}
