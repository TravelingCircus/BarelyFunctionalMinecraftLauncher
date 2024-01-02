namespace Utils;

internal readonly struct VersionRange
{
    internal readonly Option<Version> Min;
    internal readonly Option<Version> Max;

    //TODO IMPLEMENT
    
    internal bool Contains(Version version)
    {
        if (Min.IsSome(out Version min) && version < min) return false;
        if (Max.IsSome(out Version max) && version > max) return false;
        return true;
    }
    
    private readonly struct VersionBound
    {
        public readonly bool IsLimited;
        public readonly bool IsExclusive;
        public readonly Version Version;
    }
}