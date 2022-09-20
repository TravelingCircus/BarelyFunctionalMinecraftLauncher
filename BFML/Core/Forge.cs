using System;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using FileClient;

namespace BFML.Core;

public sealed class Forge
{
    private readonly MinecraftPath _minecraftPath;
    private readonly MVersion _version;
    private readonly BFMLFileClient _fileClient;

    public Forge(MinecraftPath minecraftPath, MVersion version, BFMLFileClient fileClient)
    {
        _minecraftPath = minecraftPath;
        _version = version;
        _fileClient = fileClient;
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