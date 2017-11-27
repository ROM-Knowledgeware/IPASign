using System;

namespace IPALibrary.MachO
{
    [Flags]
    public enum MachHeaderFlags : uint
    {
        NoUndefinedReferences = 0x00000001, // MH_NOUNDEFS
        IncrementalLink = 0x00000002,       // MH_INCRLINK
        DynamicLink = 0x00000004,           // MH_DYLDLINK
        BindAtLoad = 0x00000008,            // MH_BINDATLOAD
        Prebound = 0x00000010,              // MH_PREBOUND
        SplitSegments = 0x00000020,         // MH_SPLIT_SEGS
        LazyInit = 0x00000040,              // MH_LAZY_INIT
        TwoLevelNamespace = 0x00000080,     // MH_TWOLEVEL
        ForceFlatNamespace = 0x00000100,    // MH_FORCE_FLAT
        NoMultipleDefinitions = 0x00000200, // MH_NOMULTIDEFS
        NoFixPrebinding = 0x00000400,       // MH_NOFIXPREBINDING
        Prebindable = 0x00000800,           // MH_PREBINDABLE
        AllModsBound = 0x00001000,          // MH_ALLMODSBOUND
        SubsectionsViaSymbols = 0x00002000, // MH_SUBSECTIONS_VIA_SYMBOLS
        Canonical = 0x00004000,             // MH_CANONICAL
        WeakDefines = 0x00008000,           // MH_WEAK_DEFINES
        BindsToWeak = 0x00010000,           // MH_BINDS_TO_WEAK
        AllowStackExecution = 0x00020000,   // MH_ALLOW_STACK_EXECUTION
        LoadAtRandomAddress = 0x00200000,   // MH_PIE
    }
}
