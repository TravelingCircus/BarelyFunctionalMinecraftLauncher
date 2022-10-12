namespace Common.Models;

[Serializable]
public sealed class ConfigurationVersion
{
    public int MajorVersion;
    public int MinorVersion;
    public string Changelog;

    public ConfigurationVersion()
    {
        
    }

    public ConfigurationVersion(int majorVersion, int minorVersion, string changelog)
    {
        MajorVersion = majorVersion;
        MinorVersion = minorVersion;
        Changelog = changelog;
    }
}