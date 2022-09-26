namespace CommonData.Models;

[Serializable]
public sealed class ConfigurationVersion
{
    public readonly int MajorVersion;
    public readonly int MinorVersion;
    public readonly string Changelog;

    public ConfigurationVersion(int majorVersion, int minorVersion, string changelog)
    {
        MajorVersion = majorVersion;
        MinorVersion = minorVersion;
        Changelog = changelog;
    }
}