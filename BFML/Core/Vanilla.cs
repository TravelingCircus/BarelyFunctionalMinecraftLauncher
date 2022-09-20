using CmlLib.Core;
using CmlLib.Core.Version;

namespace BFML.Core;

public sealed class Vanilla
{
    public readonly MVersion Version;
    private readonly MinecraftPath _path;

    public Vanilla(MVersion version, MinecraftPath path)
    {
        Version = version;
        _path = path;
    }
}