namespace CommonData.Models;

[Serializable]
public sealed class LaunchConfiguration
{
    public readonly string VanillaVersion;
    public readonly string ForgeVersion;
    public readonly string ModsChecksum;

    public LaunchConfiguration(string vanillaVersion, string forgeVersion, string modsChecksum)
    {
        VanillaVersion = vanillaVersion;
        ForgeVersion = forgeVersion;
        ModsChecksum = modsChecksum;
    }
}