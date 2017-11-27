
namespace IPALibrary.MachO
{
    public enum FileType : uint
    {
        Object = 0x00000001,              // MH_OBJECT
        Executable = 0x00000002,          // MH_EXECUTE
        FixedVMLibrary = 0x00000003,      // MH_FVMLIB
        CoreFile = 0x00000004,            // MH_CORE
        PreloadedExecutable = 0x00000005, // MH_PRELOAD
        DynamicLibrary = 0x00000006,      // MH_DYLIB
        DynamicLinkEditor = 0x0000007,    // MH_DYLINKER
        Bundle = 0x00000008,              // MH_BUNDLE
        DynamicLibraryStub = 0x00000009,  // MH_DYLIB_STUB
        DebugSymbols = 0x0000000A,        // MH_DSYM
        KExtBundle = 0x0000000B,          // MH_KEXT_BUNDLE
    }
}
