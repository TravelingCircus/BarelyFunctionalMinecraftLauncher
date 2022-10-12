using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using Common.Models;
using TCPFileClient;

namespace BFML.Core;

public sealed class Forge
{
    public readonly MVersion Version;
    private readonly MinecraftPath _minecraftPath;

    public Forge(MVersion version, MinecraftPath minecraftPath)
    {
        Version = version;
        _minecraftPath = minecraftPath;
    }

    public bool IsInstalled(LaunchConfiguration launchConfiguration)
    {
        string forgeVersionPath = _minecraftPath.Versions + $@"\{launchConfiguration.ForgeVersion}";
        return new DirectoryInfo(forgeVersionPath).Exists;
    }

    public Task Install(string archivePath, LaunchConfiguration launchConfiguration)
    {
        throw new NotImplementedException();
        return Task.CompletedTask;
    }
}