using System.IO;
using Utils;
using Version = Utils.Version;

namespace BFML.Core;

internal readonly struct GameConfiguration
{
    internal readonly bool IsModded;
    internal readonly Version Vanilla;
    internal readonly Forge Forge;
    internal readonly ModPack ModPack;

    public GameConfiguration(bool isModded, Version vanilla, Forge forge, ModPack modPack)
    {
        IsModded = isModded;
        Vanilla = vanilla;
        Forge = forge;
        ModPack = modPack;
    }

    public Result<bool> IsValid()
    {
        if (!IsModded) return Result<bool>.Ok(true);

        Version vanillaVersion = new Version(Vanilla.ToString());
        if (Forge.TargetVanillaVersion != vanillaVersion)
        {
            return Result<bool>.Err(new InvalidDataException(
                $"Forge version mismatch. Forge:{Forge.SubVersion} | Vanilla:{vanillaVersion}"));
        }
        
        if (ModPack.VanillaVersion != vanillaVersion)
        {
            return Result<bool>.Err(new InvalidDataException(
                $"ModPack version mismatch. ModPack:{ModPack.VanillaVersion} | Vanilla:{vanillaVersion}"));
        }

        return Result<bool>.Ok(true);
    }
}