namespace CommonData.Models;

[Serializable]
public sealed class LaunchConfiguration
{
    public string VanillaVersion;
    public string ForgeVersion;
    public string ModsChecksum;

    public LaunchConfiguration()
    {
        
    }

    public LaunchConfiguration(string vanillaVersion, string forgeVersion, string modsChecksum)
    {
        VanillaVersion = vanillaVersion;
        ForgeVersion = forgeVersion;
        ModsChecksum = modsChecksum;
    }
}