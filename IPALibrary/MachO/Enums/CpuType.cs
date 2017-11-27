
namespace IPALibrary.MachO
{
    public enum CpuType : uint
    {
        Vax = 1,
        Romp = 2,
        NS32032 = 4,
        NS32332 = 5,
        MC680x0 = 6,
        I386 = 7,
        MIPS = 8,
        NS32532 = 9,
        HPPA = 11,
        Arm = 12,
        MC88000 = 13,
        Sparc = 14,
        I860BigEndian = 15,
        I860LittleEndian = 16,
        RS6000 = 17,
        MC98000 = 18,
        PowerPC = 18,
        Veo = 255,
        CPU_TYPE_X86_64 = 0x01000007,
        Arm64 = 0x0100000C,
        PowerPC64 = 0x01000012,
    }
}
