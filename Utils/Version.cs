namespace Utils;

public readonly struct Version
{
    public IReadOnlyCollection<int> Parts => _parts;
    private readonly int[] _parts;

    public Version(string versionText)
    {
        string[] partsText = versionText.Split('.');
        _parts = partsText.Select(int.Parse).ToArray();
    }

    public Version(params int[] parts)
    {
        _parts = parts;
    }

    //TODO IMPLEMENT OPERATORS
    
    public static bool operator > (Version a, Version b)
    {
        bool greater = false;
        for (int i = 0; i < MathF.Max(a.Parts.Count, b.Parts.Count); i++)
        {
            
        }
        return greater;
    }

    public static bool operator < (Version a, Version b)
    {
        return false;
    }

    public override string ToString()
    {
        return String.Join('.', Parts);
    }
}