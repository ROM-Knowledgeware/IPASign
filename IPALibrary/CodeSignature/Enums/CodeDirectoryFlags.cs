using System;

namespace IPALibrary.CodeSignature
{
    [Flags]
    public enum CodeDirectoryFlags : uint
    {
        Valid = 0x00000001,                    // CS_VALID
        AdHoc = 0x00000002,                    // CS_ADHOC
        GetTaskAllow = 0x00000004,             // CS_GET_TASK_ALLOW
        Installer = 0x00000008,                // CS_INSTALLER
        Hard = 0x00000100,                     // CS_HARD
        Kill = 0x00000200,                     // CS_KILL
        CheckExpiration = 0x00000400,          // CS_CHECK_EXPIRATION
        Restrict = 0x00000800,                 // CS_RESTRICT
        Enforcement = 0x00001000,              // CS_ENFORCEMENT
        RequireLibraryValidation = 0x00002000, // CS_REQUIRE_LV
        EntitlementsValidated = 0x00004000,    // CS_ENTITLEMENTS_VALIDATED
    }
}
