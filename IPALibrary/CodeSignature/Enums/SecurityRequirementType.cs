
namespace IPALibrary.CodeSignature
{
    /// <summary>
    /// SecRequirementType
    /// </summary>
    public enum SecurityRequirementType : uint
    {
        HostRequirementType = 0x00000001,       // kSecHostRequirementType
        GuestRequirementType = 0x00000002,      // kSecGuestRequirementType
        DesignatedRequirementType = 0x00000003, // kSecDesignatedRequirementType
        LibraryRequirementType = 0x00000004,    // kSecLibraryRequirementType
        PluginRequirementType = 0x00000005,     // kSecPluginRequirementType
    }
}
