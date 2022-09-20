using System;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using FileClient;

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

    public bool IsInstalled()
    {
        throw new NotImplementedException();
    }

    public Task Install(string archivePath)
    {
        throw new NotImplementedException();
        return Task.CompletedTask;
    }
}