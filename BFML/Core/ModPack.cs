using CmlLib.Core.Version;
using Common.Models;

namespace BFML.Core;

internal sealed class ModPack
{
    internal MVersion TargetVanillaVersion => new MVersion(_manifest.VanillaVersion);
    private readonly ModPackManifest _manifest;

    internal ModPack(ModPackManifest manifest)
    {
        _manifest = manifest;
    }
}