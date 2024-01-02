namespace Common.Models;

public readonly struct ModPackManifest
{
    public readonly Guid Guid;
    public readonly string Name;
    public readonly string ModPackVersion;
    public readonly string Changelog;
    public readonly string ForgeVersion;
    public readonly string VanillaVersion;
    public readonly ulong ModsChecksum;
}
