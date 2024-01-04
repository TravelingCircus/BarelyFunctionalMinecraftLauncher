namespace Utils;

public readonly struct VersionRange
{
    public readonly VersionBound Min;
    public readonly VersionBound Max;

    private VersionRange(Version min, Version max, bool isMinInclusive, bool isMaxInclusive)
    {
        Min = new VersionBound(min, isMinInclusive);
        Max = new VersionBound(max, isMaxInclusive);
    }

    public static VersionRange Between(Version min, Version max, bool isMinInclusive = true, bool isMaxInclusive = true)
    {
        if (max < min) throw new ArgumentException($"Max {max} must be greater than Min {min}");
        return new VersionRange(min, max, isMinInclusive, isMaxInclusive);
    }
    
    public static VersionRange From(Version min, bool isMinInclusive = true) 
    {
        return new VersionRange(min, Version.None, isMinInclusive, true);
    }
    
    public static VersionRange To(Version max, bool isMaxInclusive = true) 
    {
        return new VersionRange(Version.None, max, true, isMaxInclusive);
    }

    public static VersionRange Between(string range)
    {
        bool isMinInclusive = IsBracketInclusive(range[0]);
        bool isMaxInclusive = IsBracketInclusive(range[^1]);

        string[] versionStrings = range.Substring(1, range.Length - 2).Split(',');
        Version min = new Version(versionStrings[0]);
        Version max = new Version(versionStrings[1]);

        return Between(min, max, isMinInclusive, isMaxInclusive);
    }
    
    public static VersionRange From(string range)
    {
        bool isMinInclusive = IsBracketInclusive(range[0]);

        string[] versionStrings = range.Substring(1, range.Length - 2).Split(',');
        Version min = new Version(versionStrings[0]);

        return From(min, isMinInclusive);
    }
    
    public static VersionRange To(string range)
    {
        bool isMaxInclusive = IsBracketInclusive(range[^1]);
        
        string[] versionStrings = range.Substring(1, range.Length - 2).Split(',');
        Version max = new Version(versionStrings[1]);

        return To(max, isMaxInclusive);
    }

    public bool Contains(Version version)
    {
        bool minConditionPassed = !Min.IsLimited 
                                  || (Min.IsInclusive 
                                        ? version >= Min.Version 
                                        : version > Min.Version);
        bool maxConditionPassed = !Max.IsLimited 
                                  || (Max.IsInclusive 
                                      ? version <= Max.Version 
                                      : version < Max.Version);

        return minConditionPassed && maxConditionPassed;
    }
    
    public override string ToString()
    {
        char minBracket = Min.IsLimited 
            ? Min.IsInclusive ? '[' : '('
            : '(';
        char maxBracket = Max.IsLimited 
            ? Max.IsInclusive ? ']' : ')'
            : ')';
        return minBracket + Min.ToString() + ',' + Max + maxBracket;
    }

    private static bool IsBracketInclusive(char bracket)
    {
        return bracket switch
        {
            '(' or ')' => false,
            '[' or ']' => true,
            _ => throw new ArgumentOutOfRangeException(
                bracket.ToString(), 
                "Bracket required:"
                + "\nExclusive: '(' or ')';"
                + "\nInclusive: '[' or ']'.")
        };
    }

    public readonly struct VersionBound
    {
        public readonly bool IsLimited;
        public readonly Version Version;
        public readonly bool IsInclusive;

        public VersionBound(Version version, bool isInclusive)
        {
            Version = version;
            IsInclusive = isInclusive;
            IsLimited = IsVersionZero(version) || version != Version.None;
        }

        public override string ToString()
        {
            return IsLimited 
                ? Version.ToString() 
                : string.Empty;
        }

        private static bool IsVersionZero(Version version) => version.Parts.Count == 1 && version.Parts[0] == 0;
    }
}