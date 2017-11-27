
namespace IPALibrary.MachO
{
    public enum LoadCommandType : uint
    {
        Segment = 0x00000001,                // LC_SEGMENT
        SymbolTable = 0x00000002,            // LC_SYMTAB
        Thread = 0x00000004,                 // LC_THREAD
        UnixThread = 0x00000005,             // LC_UNIXTHREAD
        LoadFixedVMLibrary = 0x00000006,     // LC_LOADFVMLIB
        DynamicSymbolTable = 0x0000000B,     // LC_DYSYMTAB
        LoadDynamicLibrary = 0x0000000C,     // LC_LOAD_DYLIB
        IDDynamicLibrary = 0x0000000D,       // LC_ID_DYLIB
        LoadDynamicLinker = 0x0000000E,      // LC_LOAD_DYLINKER
        IDDynamicLinker = 0x0000000F,        // LC_ID_DYLINKER
        PreboundDynamicLibrary = 0x00000010, // LC_PREBOUND_DYLIB
        Routines = 0x00000011,               // LC_ROUTINES
        SubFramework = 0x00000012,           // LC_SUB_FRAMEWORK
        SubUmbrella = 0x00000013,            // LC_SUB_UMBRELLA
        SubClient = 0x00000014,              // LC_SUB_CLIENT
        SubLibrary = 0x00000015,             // LC_SUB_LIBRARY
        TwoLevelHints = 0x00000016,          // LC_TWOLEVEL_HINTS
        Segment64 = 0x00000019,              // LC_SEGMENT_64
        Routines64 = 0x0000001A,             // LC_ROUTINES_64
        UUID = 0x0000001B,                   // LC_UUID
        CodeSignature = 0x0000001D,          // LC_CODE_SIGNATURE
        FunctionStarts = 0x00000026,         // LC_FUNCTION_STARTS
        EncryptionInfo = 0x00000021,         // LC_ENCRYPTION_INFO
        VersionMinIPhoneOS = 0x00000025,     // LC_VERSION_MIN_IPHONEOS
        DataInCode = 0x00000029,             // LC_DATA_IN_CODE
        SourceVersion = 0x0000002A,          // LC_SOURCE_VERSION
        EncryptionInfo64 = 0x0000002C,       // LC_ENCRYPTION_INFO_64
        LoadWeakDynamicLibrary = 0x80000018, // LC_LOAD_WEAK_DYLIB
        DynamicLinkerInfoOnly = 0x80000022,  // LC_DYLD_INFO_ONLY
        MainEntryPoint = 0x80000028,         // LC_MAIN
    }
}
