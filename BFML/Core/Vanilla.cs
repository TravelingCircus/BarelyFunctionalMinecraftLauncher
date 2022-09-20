using CmlLib.Core;
using CmlLib.Core.Version;

namespace BFML.Core;

public sealed class Vanilla
{
    private readonly MVersion _version;
    private readonly MinecraftPath _path;

    public Vanilla(MinecraftPath path, MVersion version)
    {
        _version = version;
        _path = path;
    }
}