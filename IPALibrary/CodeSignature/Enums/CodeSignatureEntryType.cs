using System;

namespace IPALibrary.CodeSignature
{
    public enum CodeSignatureEntryType : uint
    {
        CodeDirectory = 0x00000000, // CSSLOT_CODEDIRECTORY, The signature of this entry type will be CSMAGIC_CODEDIRECTORY
        Requirements = 0x00000002,  // CSSLOT_REQUIREMENTS,  The signature of this entry type will be CSMAGIC_REQUIREMENTS
        Entitlements = 0x00000005,  // CSSLOT_ENTITLEMENTS,  The signature of this entry type will be CSMAGIC_EMBEDDED_ENTITLEMENTS
        AlternateCodeDirectory1 = 0x00001000, // CSSLOT_ALTERNATE_CODEDIRECTORIES, The signature of this entry type will be CSMAGIC_CODEDIRECTORY
        CmsSignature = 0x00010000,  // CSSLOT_SIGNATURESLOT, The signature of this entry type will be CSMAGIC_BLOBWRAPPER
    }
}
