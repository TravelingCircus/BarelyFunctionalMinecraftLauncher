using System;
using System.IO;
using BFML.Repository;
using CmlLib.Core.Version;
using Utils;
using Version = Utils.Version;

namespace BFML.Core;

[Serializable]
internal struct LaunchConfiguration
{
    internal MVersion VanillaVersion = new MVersion("1.18.2");
    internal bool IsModded = false;
    internal Forge ForgeVersion = null;
    internal ModPack ModPack = null;
    internal int DedicatedRam = 4096;
    internal bool FullScreen = false;
    internal string Nickname = "ABOBA";
    internal FileValidation Validation = FileValidation.Full;

    public LaunchConfiguration() { }

    public Result<bool> IsValid()
    {
        if (DedicatedRam < 1024) return Result<bool>.Err(new InvalidDataException("Can't launch with less than 1GB of RAM"));
        if (string.IsNullOrEmpty(Nickname)) return Result<bool>.Err(new InvalidDataException("Can't launch with empty nickname"));

        if (!IsModded) return Result<bool>.Ok(true);

        Version vanillaVersion = new Version(VanillaVersion.ToString());
        if (ForgeVersion.TargetVanillaVersion != vanillaVersion)
        {
            return Result<bool>.Err(new InvalidDataException(
                $"Forge version mismatch. Forge:{ForgeVersion.SubVersion} | Vanilla:{vanillaVersion}"));
        }
        
        if (ModPack.VanillaVersion != vanillaVersion)
        {
            return Result<bool>.Err(new InvalidDataException(
                $"ModPack version mismatch. ModPack:{ModPack.VanillaVersion} | Vanilla:{vanillaVersion}"));
        }
        
        return Result<bool>.Ok(true);
    }
}