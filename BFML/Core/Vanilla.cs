using CmlLib.Core;
using CmlLib.Core.Version;

namespace BFML.Core;

public class Vanilla
{
    public readonly MVersion Version;
    private readonly MinecraftPath _path;

    public Vanilla(MinecraftPath path, string versionString)
    {
        _path = path;
        Version = new MVersion(versionString);
    }
}