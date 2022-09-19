using System;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;

namespace BFML.Core;

public class Forge
{
    private readonly MinecraftPath _minecraftPath;
    private readonly MVersion _version;

    public Forge(MinecraftPath minecraftPath, MVersion version)
    {
        _minecraftPath = minecraftPath;
        _version = version;
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