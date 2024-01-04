namespace Utils;

public readonly struct Version : IEquatable<Version>, IComparable<Version>
{
    public static readonly Version Empty = new Version();
    
    public IReadOnlyList<byte> Parts => _parts;
    private readonly byte[] _parts;
    
    public Version()
    {
        _parts = Array.Empty<byte>();
    }
    
    public Version(params byte[] parts)
    {
        _parts = parts;
    }

    public Version(string versionText)
    {
        string[] partsText = versionText.Split('.');
        _parts = partsText.Select(byte.Parse).ToArray();
    }
    
    public static bool operator >= (Version a, Version b) => a > b || a == b;

    public static bool operator <= (Version a, Version b) => a < b || a == b;
    
    public static bool operator > (Version a, Version b) => a.CompareTo(b) > 0;

    public static bool operator <(Version a, Version b) => a.CompareTo(b) < 0;
    
    public static bool operator == (Version a, Version b) => a.Equals(b);

    public static bool operator !=(Version a, Version b) => !a.Equals(b);

    public override bool Equals(object? obj) => obj is Version other && Equals(other);

    public override int GetHashCode() => _parts.GetHashCode();

    public override string ToString() => _parts is null 
        ? string.Empty 
        : string.Join('.', Parts);

    public bool Equals(Version other) => CompareTo(other) == 0;

    public int CompareTo(Version other)
    {
        if (_parts is null || other._parts is null) throw new NullReferenceException();
        
        int partsLength = _parts.Length;
        int otherPartsLength = other._parts.Length;
        for (int i = 0; i < Math.Max(partsLength, otherPartsLength); i++)
        {
            if (i < partsLength && i < otherPartsLength)
            {
                if (_parts[i] > other._parts[i]) return 1;
                if (_parts[i] < other._parts[i]) return -1;
            }
            else if (i < partsLength && i >= otherPartsLength)
            {
                if (_parts[i] != 0) return 1;
            }
            else if (i >= partsLength && i < otherPartsLength)
            {
                if (other._parts[i] != 0) return -1;
            }
        }

        return 0;
    }
}