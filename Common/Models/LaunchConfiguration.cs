namespace Common.Models;

[Serializable]
public sealed class LaunchConfiguration
{
    public string VanillaVersion;
    public string ForgeVersion;
    public string ModsChecksum;
    public int Ram;
    public bool FullScreen = false;
    public string Nickname = "ABOBA";

    public LaunchConfiguration(string vanillaVersion, string forgeVersion, string modsChecksum, int ram)
    {
        VanillaVersion = vanillaVersion;
        ForgeVersion = forgeVersion;
        ModsChecksum = modsChecksum;
        Ram = ram;
    }

    public bool IsValid()
    {
        return true; //TODO Validate configuration
    }
}