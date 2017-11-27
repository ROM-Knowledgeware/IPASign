
namespace IPALibrary.CodeSignature
{
    /// <summary>
    /// Defined in requirement.h
    /// https://opensource.apple.com/source/libsecurity_codesigning/libsecurity_codesigning-55037.15/lib/requirement.h.auto.html
    /// </summary>
    public enum MatchOperationName : uint
    {
        Exists = 0,
        Equal = 1,
        Contains = 2,
        BeginsWith = 3,
        EndsWith = 4,
        LessThan = 5,
        GreaterThan = 6,
        LessThanOrEqual = 7,
        GreaterThanOrEqual = 8,
    }
}
